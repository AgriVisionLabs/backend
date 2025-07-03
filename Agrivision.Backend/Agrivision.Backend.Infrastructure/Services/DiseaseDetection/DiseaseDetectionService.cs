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

        for (int attempt = 0; attempt < 3; attempt++)
        {
            var probe = await httpClient.GetAsync(imageUrl);
            if (probe.IsSuccessStatusCode &&
                probe.Content.Headers.ContentType?.MediaType?.StartsWith("image") == true)
            {
                await Task.Delay(1000); // Optional buffer
                break;
            }

            await Task.Delay(2000);
            if (attempt == 2)
            {
                logger.LogCritical("Image not accessible after 3 attempts: {Url}", imageUrl);
                return null;
            }
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

            var getRequest = new HttpRequestMessage(HttpMethod.Get, $"{diseaseDetectionSettings.Value.ImagePredictionUrl}/{eventId}?stream=false");
            getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", diseaseDetectionSettings.Value.ImageDetectionModelToken);

            var resultResponse = await httpClient.SendAsync(getRequest);
            var resultString = await resultResponse.Content.ReadAsStringAsync();

            if (!resultResponse.IsSuccessStatusCode ||
                resultString.StartsWith("<!DOCTYPE html>") ||
                resultString.Contains("502 Bad Gateway"))
            {
                logger.LogCritical("Prediction failed or returned invalid HTML for image: {Url}", imageUrl);
                return null;
            }

            string? dataLine = resultString.Split('\n').FirstOrDefault(line => line.StartsWith("data:"));
            string? eventLine = resultString.Split('\n').FirstOrDefault(line => line.StartsWith("event:"));

            if (dataLine is null)
            {
                logger.LogCritical("SSE response missing data line for image: {Url}", imageUrl);
                return null;
            }

            string jsonData = dataLine["data:".Length..].Trim();

            if (eventLine?.Trim() == "event: error" || string.IsNullOrWhiteSpace(jsonData) || jsonData == "null")
            {
                logger.LogCritical("Gradio model returned error/null for image: {Url}", imageUrl);
                return null;
            }

            using var parsedJson = JsonDocument.Parse(jsonData);
            if (parsedJson.RootElement.ValueKind != JsonValueKind.Array)
            {
                logger.LogCritical("Expected JSON array but got: {Kind} for image: {Url}", parsedJson.RootElement.ValueKind, imageUrl);
                return null;
            }

            var predictions = JsonSerializer.Deserialize<List<DiseasePredictionResponse>>(
                jsonData,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (predictions is null || predictions.Count == 0)
            {
                logger.LogCritical("No predictions returned for image: {Url}", imageUrl);
                return null;
            }

            var first = predictions.FirstOrDefault();
            logger.LogCritical("Final prediction: {@Prediction}", first);
            return first;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unhandled exception during prediction for image: {Url}", imageUrl);
            return null;
        }
    }
}