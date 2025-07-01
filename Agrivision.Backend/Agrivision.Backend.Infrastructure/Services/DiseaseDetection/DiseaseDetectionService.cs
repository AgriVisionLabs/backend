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

public class DiseaseDetectionService(HttpClient httpClient, IOptions<DiseaseDetectionSettings> diseaseDetectionSettings, IOptions<ServerSettings> serverSettings, ILogger<DiseaseDetectionService> logger) : IDiseaseDetectionService
{
    public async Task<DiseasePredictionResponse> PredictImageAsync(string filename)
    {
        string imageUrl = $"{serverSettings.Value.BaseUrl}/uploads/{filename}";
        
        logger.LogCritical("Image URL: " + imageUrl);

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
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", diseaseDetectionSettings.Value.ImageDetectionModelToken);
        
        try
        {
            // 1) POST → get event_id
            var response = await httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            
            using var doc = JsonDocument.Parse(responseString);
            var eventId = doc.RootElement.GetProperty("event_id").GetString();
            
            logger.LogCritical($"Extracted Event ID: {eventId}");
                
            // 2) GET the *JSON-only* result by disabling SSE with ?stream=false
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", diseaseDetectionSettings.Value.ImageDetectionModelToken);
            var resultResponse = await httpClient.GetAsync($"{diseaseDetectionSettings.Value.ImagePredictionUrl}/{eventId}?stream=false");
            var resultString = await resultResponse.Content.ReadAsStringAsync();

            if (!resultResponse.IsSuccessStatusCode)
            {
                logger.LogError("Prediction failed. Status: {StatusCode}, Body: {Body}", resultResponse.StatusCode, resultString);
                throw new HttpRequestException($"Prediction failed: {resultResponse.StatusCode}");
            }

            logger.LogInformation("Prediction succeeded.");

            string? dataLine = resultString.Split('\n').FirstOrDefault(line => line.StartsWith("data:"));
            if (dataLine is null)
                throw new InvalidOperationException("Invalid SSE response format: missing data line.");

            string jsonData = dataLine["data:".Length..].Trim();

            var predictions = JsonSerializer.Deserialize<List<DiseasePredictionResponse>>(
                jsonData,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? throw new InvalidOperationException("Failed to deserialize prediction response.");
            
            logger.LogCritical("final: " + predictions.FirstOrDefault().Label + predictions.FirstOrDefault().Confidences);

            return predictions.FirstOrDefault() ?? throw new InvalidOperationException("No prediction results returned.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during prediction.");
            throw;
        }
    }
}