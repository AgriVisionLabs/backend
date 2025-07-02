using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Agrivision.Backend.Application.Services.DiseaseDetection;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Models;
using Agrivision.Backend.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Infrastructure.Services.DiseaseDetection;

public class DiseaseDetectionService(
    HttpClient httpClient,
    IOptions<DiseaseDetectionSettings> diseaseDetectionSettings,
    IOptions<ServerSettings> serverSettings,
    ILogger<DiseaseDetectionService> logger
) : IDiseaseDetectionService
{
    public async Task<DiseasePredictionResponse?> PredictImageAsync(string filename)
    {
        string imageUrl = $"{serverSettings.Value.BaseUrl}/uploads/{filename}";
        logger.LogCritical("Image URL: " + imageUrl);

        // ⏳ Ensure image is publicly reachable before prediction
        for (int attempt = 0; attempt < 3; attempt++)
        {
            var probe = await httpClient.GetAsync(imageUrl);
            if (probe.IsSuccessStatusCode &&
                probe.Content.Headers.ContentType?.MediaType?.StartsWith("image") == true)
            {
                logger.LogInformation("Image is available and ready for prediction.");
                break;
            }

            logger.LogWarning("Image not ready yet. Attempt {Attempt}/3. Status: {StatusCode}", attempt + 1, probe.StatusCode);
            await Task.Delay(1000); // Wait 1s before retry
            if (attempt == 2)
                throw new InvalidOperationException($"Image not accessible for prediction: {imageUrl}");
        }

        var requestBody = new
        {
            data = new[]
            {
                new
                {
                    path = imageUrl,
                    meta = new { _type = "gradio.FileData" }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        logger.LogCritical(diseaseDetectionSettings.Value.ImagePredictionUrl);
        logger.LogCritical(diseaseDetectionSettings.Value.ImageDetectionModelToken);

        var request = new HttpRequestMessage(HttpMethod.Post, diseaseDetectionSettings.Value.ImagePredictionUrl)
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", diseaseDetectionSettings.Value.ImageDetectionModelToken);

        try
        {
            var response = await httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var eventId = doc.RootElement.GetProperty("event_id").GetString();
            logger.LogCritical($"Extracted Event ID: {eventId}");

            var getRequest = new HttpRequestMessage(HttpMethod.Get, $"{diseaseDetectionSettings.Value.ImagePredictionUrl}/{eventId}?stream=false");
            getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", diseaseDetectionSettings.Value.ImageDetectionModelToken);

            var resultResponse = await httpClient.SendAsync(getRequest);
            var resultString = await resultResponse.Content.ReadAsStringAsync();

            if (!resultResponse.IsSuccessStatusCode)
            {
                logger.LogError("Prediction failed. Status: {StatusCode}, Body: {Body}", resultResponse.StatusCode, resultString);
                throw new HttpRequestException($"Prediction failed: {resultResponse.StatusCode}");
            }

            logger.LogCritical("Raw resultString: " + resultString);

            if (resultString.StartsWith("<!DOCTYPE html>") || resultString.Contains("502 Bad Gateway"))
            {
                logger.LogError("Received non-JSON response: {Body}", resultString);
                throw new InvalidOperationException("Unexpected non-JSON response from model server.");
            }

            string? dataLine = resultString.Split('\n').FirstOrDefault(line => line.StartsWith("data:"));
            string? eventLine = resultString.Split('\n').FirstOrDefault(line => line.StartsWith("event:"));

            if (dataLine is null)
                throw new InvalidOperationException("Invalid SSE response format: missing data line.");

            string jsonData = dataLine["data:".Length..].Trim();
            logger.LogCritical("Extracted jsonData: " + jsonData);

            if (eventLine?.Trim() == "event: error" || string.IsNullOrWhiteSpace(jsonData) || jsonData == "null")
            {
                logger.LogError("Gradio model returned an error event or null prediction.");
                throw new InvalidOperationException("Model inference failed or returned no data.");
            }

            try
            {
                using var parsedJson = JsonDocument.Parse(jsonData);
                if (parsedJson.RootElement.ValueKind != JsonValueKind.Array)
                {
                    logger.LogError("Expected JSON array but got: {Kind}", parsedJson.RootElement.ValueKind);
                    throw new InvalidOperationException("Prediction response is not a JSON array.");
                }

                var predictions = JsonSerializer.Deserialize<List<DiseasePredictionResponse>>(
                    jsonData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? throw new InvalidOperationException("Failed to deserialize prediction response.");

                var first = predictions.FirstOrDefault();
                logger.LogCritical("final: {@Prediction}", first);

                return first ?? throw new InvalidOperationException("No prediction results returned.");
            }
            catch (JsonException jsonEx)
            {
                logger.LogError(jsonEx, "JSON deserialization failed. jsonData: {jsonData}", jsonData);
                throw new InvalidOperationException("Failed to parse prediction JSON.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during prediction.");
            throw;
        }
    }
}