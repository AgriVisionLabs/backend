using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Agrivision.Backend.Application.Services.DiseaseDetection;
using Agrivision.Backend.Application.Services.Files;
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
    ILogger<DiseaseDetectionService> logger,
    IFileUploadService fileUploadService,
    IVideoProcessingService videoProcessingService,
    IImageProcessingService imageProcessingService
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

    // return composite image URL with all annotated frames and health counts
    public async Task<(string compositeImageUrl, int healthyCount, int infectedCount)> PredictVideoAsync(string filename)
    {
        try
        {
            logger.LogInformation("Starting video processing for file: {Filename}", filename);
            
            // extract frames from video
            var frameFilenames = await videoProcessingService.ExtractVideoFramesAsync(filename, intervalSeconds: 10);
            if (!frameFilenames.Any())
            {
                logger.LogCritical("No frames extracted from video: {Filename}", filename);
                return ("", 0, 0);
            }

            // process each frame through disease detection
            var annotatedFrames = new List<(string imagePath, string label, double confidence)>();
            var healthyCount = 0;
            var infectedCount = 0;

            foreach (var frameFilename in frameFilenames)
            {
                var frameResult = await ProcessSingleFrameAsync(frameFilename);
                if (frameResult != null)
                {
                    annotatedFrames.Add(frameResult.Value);
                    
                    // count healthy vs infected frames
                    if (frameResult.Value.label.Contains("healthy", StringComparison.OrdinalIgnoreCase))
                    {
                        healthyCount++;
                    }
                    else
                    {
                        infectedCount++;
                    }
                }
            }

            if (!annotatedFrames.Any())
            {
                logger.LogCritical("No frames were successfully processed for video: {Filename}", filename);
                return ("", 0, 0);
            }

            // create composite image
            var compositeFilename = $"composite_{Guid.NewGuid():N}.jpg";
            await imageProcessingService.CreateCompositeImageAsync(annotatedFrames, compositeFilename);
            var compositeUrl = $"{serverSettings.Value.BaseUrl}/results/{compositeFilename}";

            // cleanup intermediate files
            Cleanup(filename, frameFilenames);

            logger.LogInformation("Video processing completed. Composite image: {CompositeUrl}, Healthy: {HealthyCount}, Infected: {InfectedCount}", 
                compositeUrl, healthyCount, infectedCount);

            return (compositeUrl, healthyCount, infectedCount);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unhandled exception during video prediction for: {Filename}", filename);
            
            // cleanup on error
            try
            {
                Cleanup(filename, new List<string>());
            }
            catch (Exception cleanupEx)
            {
                logger.LogWarning(cleanupEx, "Failed to cleanup files after error");
            }
            
            return ("", 0, 0);
        }
    }

    private async Task<(string imagePath, string label, double confidence)?> ProcessSingleFrameAsync(string frameFilename)
    {
        try
        {
            var frameImageUrl = $"{serverSettings.Value.BaseUrl}/uploads/frames/{frameFilename}";
            
            // wait for frame to be accessible
            for (int attempt = 0; attempt < 3; attempt++)
            {
                var probe = await httpClient.GetAsync(frameImageUrl);
                if (probe.IsSuccessStatusCode && 
                    probe.Content.Headers.ContentType?.MediaType?.StartsWith("image") == true)
                {
                    await Task.Delay(1000);
                    break;
                }

                await Task.Delay(2000);
                if (attempt == 2)
                {
                    logger.LogWarning("Frame not accessible after 3 attempts: {Url}", frameImageUrl);
                    return null;
                }
            }

            // send to gradio api
            var requestBody = new
            {
                data = new[]
                {
                    new
                    {
                        path = frameImageUrl,
                        meta = new { _type = "gradio.FileData" }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, diseaseDetectionSettings.Value.VideoPredictionUrl)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", diseaseDetectionSettings.Value.VideoDetectionModelToken);

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5)); // 5 minute timeout
            var response = await httpClient.SendAsync(request, cts.Token);
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("API call failed with status: {StatusCode} for frame: {Url}", response.StatusCode, frameImageUrl);
                return null;
            }
            
            var responseString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseString))
            {
                logger.LogWarning("Empty response from video prediction API for frame: {Url}", frameImageUrl);
                return null;
            }

            // extract event id with better error handling
            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(responseString);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to parse JSON response for frame: {Url}", frameImageUrl);
                return null;
            }
            
            using (doc)
            {
                if (!doc.RootElement.TryGetProperty("event_id", out var eventIdProperty))
                {
                    logger.LogWarning("No event_id property found in response for frame: {Url}", frameImageUrl);
                    return null;
                }
                
                var eventId = eventIdProperty.GetString();
                if (string.IsNullOrWhiteSpace(eventId))
                {
                    logger.LogWarning("No event_id returned from video prediction API for frame: {Url}", frameImageUrl);
                    return null;
                }

                // poll for results with timeout
                var getRequest = new HttpRequestMessage(HttpMethod.Get, $"{diseaseDetectionSettings.Value.VideoPredictionUrl}/{eventId}?stream=false");
                getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", diseaseDetectionSettings.Value.VideoDetectionModelToken);

                var resultResponse = await httpClient.SendAsync(getRequest, cts.Token);
                var resultString = await resultResponse.Content.ReadAsStringAsync();

                if (!resultResponse.IsSuccessStatusCode ||
                    resultString.StartsWith("<!DOCTYPE html>") ||
                    resultString.Contains("502 Bad Gateway"))
                {
                    logger.LogWarning("Prediction failed or returned invalid HTML for frame: {Url}", frameImageUrl);
                    return null;
                }

                // parse sse output
                string? dataLine = resultString.Split('\n').FirstOrDefault(line => line.StartsWith("data:"));
                string? eventLine = resultString.Split('\n').FirstOrDefault(line => line.StartsWith("event:"));

                if (dataLine is null)
                {
                    logger.LogWarning("SSE response missing data line for frame: {Url}", frameImageUrl);
                    return null;
                }

                string jsonData = dataLine["data:".Length..].Trim();

                if (eventLine?.Trim() == "event: error" || string.IsNullOrWhiteSpace(jsonData) || jsonData == "null")
                {
                    logger.LogWarning("Gradio model returned error/null for frame: {Url}", frameImageUrl);
                    return null;
                }

                // validate and parse response structure
                JsonDocument parsedJson;
                try
                {
                    parsedJson = JsonDocument.Parse(jsonData);
                }
                catch (JsonException ex)
                {
                    logger.LogWarning(ex, "Failed to parse prediction JSON for frame: {Url}", frameImageUrl);
                    return null;
                }
                
                using (parsedJson)
                {
                    if (parsedJson.RootElement.ValueKind != JsonValueKind.Array)
                    {
                        logger.LogWarning("Expected JSON array but got: {Kind} for frame: {Url}", parsedJson.RootElement.ValueKind, frameImageUrl);
                        return null;
                    }

                    var array = parsedJson.RootElement.EnumerateArray().ToArray();
                    if (array.Length < 2)
                    {
                        logger.LogWarning("Unexpected Gradio result format for frame: {Url}", frameImageUrl);
                        return null;
                    }

                    // download annotated image
                    if (!array[0].TryGetProperty("url", out var urlProperty))
                    {
                        logger.LogWarning("No URL property found in first array element for frame: {Url}", frameImageUrl);
                        return null;
                    }

                    string? remoteAnnotatedUrl = urlProperty.GetString();
                    if (string.IsNullOrWhiteSpace(remoteAnnotatedUrl))
                    {
                        logger.LogWarning("Empty or null URL returned from video prediction for frame: {Url}", frameImageUrl);
                        return null;
                    }

                    var downloadRequest = new HttpRequestMessage(HttpMethod.Get, remoteAnnotatedUrl);
                    downloadRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", diseaseDetectionSettings.Value.VideoDetectionModelToken);

                    var downloadResponse = await httpClient.SendAsync(downloadRequest, cts.Token);
                    if (!downloadResponse.IsSuccessStatusCode)
                    {
                        logger.LogWarning("Failed to download annotated image from: {Url}", remoteAnnotatedUrl);
                        return null;
                    }

                    var annotatedBytes = await downloadResponse.Content.ReadAsByteArrayAsync();
                    if (annotatedBytes.Length == 0)
                    {
                        logger.LogWarning("Empty annotated image bytes downloaded for frame: {Url}", frameImageUrl);
                        return null;
                    }

                    // save annotated frame
                    var annotatedFilename = $"annotated_{Guid.NewGuid():N}.webp";
                    var annotatedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", annotatedFilename);
                    await File.WriteAllBytesAsync(annotatedPath, annotatedBytes);

                    // extract label and confidence
                    var detectionText = array[1].GetString();
                    string label = "Unknown";
                    double confidence = 0;
                    
                    if (!string.IsNullOrWhiteSpace(detectionText))
                    {
                        var lines = detectionText.Split('\n');
                        if (lines.Length > 1 && !string.IsNullOrWhiteSpace(lines[1]))
                        {
                            string detectionLine = lines[1];
                            int idx = detectionLine.LastIndexOf(" (confidence:", StringComparison.Ordinal);
                            if (idx > 0)
                            {
                                label = detectionLine[..idx].Trim();
                                string confidencePart = detectionLine[(idx + " (confidence:".Length)..].Trim(' ', ')');
                                if (!double.TryParse(confidencePart, out confidence))
                                {
                                    confidence = 0;
                                }
                            }
                            else
                            {
                                label = detectionLine.Trim();
                            }
                        }
                    }

                    return (annotatedFilename, label, confidence);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing frame: {FrameFilename}", frameFilename);
            return null;
        }
    }

    private void Cleanup(string originalVideoFilename, List<string> frameFilenames)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        var framesFolder = Path.Combine(uploadsFolder, "frames");

        // delete original video file
        var originalVideoPath = Path.Combine(uploadsFolder, originalVideoFilename);
        try
        {
            if (File.Exists(originalVideoPath))
            {
                File.Delete(originalVideoPath);
                logger.LogInformation("Deleted original video file: {Filename}", originalVideoFilename);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete original video file: {Filename}", originalVideoFilename);
        }

        // delete extracted frames
        foreach (var frameFilename in frameFilenames)
        {
            var framePath = Path.Combine(framesFolder, frameFilename);
            try
            {
                if (File.Exists(framePath))
                {
                    File.Delete(framePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to delete frame file: {Filename}", frameFilename);
            }
        }
    }
}