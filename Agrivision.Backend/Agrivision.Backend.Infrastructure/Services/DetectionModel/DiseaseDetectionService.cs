
using System.Text;
using Agrivision.Backend.Application.Services.DetectionModel;
using Agrivision.Backend.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Infrastructure.Services.DetectionModel;
public class DiseaseDetectionService(IOptions<DetectionModelSettings> detectionModelSettings,ILogger<DiseaseDetectionService> logger)  : IDiseaseDetectionService
{
    private readonly HttpClient _client = new HttpClient();
    private readonly string detectURL=
        $"{detectionModelSettings.Value.BaseDetectionUrl}{detectionModelSettings.Value.ModelId}?api_key={detectionModelSettings.Value.ApiKey}";
    //private readonly string detectURL =
   // $"{detectionModelSettings.Value.BaseDetectionUrl}/{detectionModelSettings.Value.ModelId}?api_key={detectionModelSettings.Value.ApiKey}";

    public async Task<string> NewDetectionAsync(string imagePath)
    {
        try
        {
            // Read image
            byte[] imageArray = await System.IO.File.ReadAllBytesAsync(imagePath);
            string encodedImage = Convert.ToBase64String(imageArray);

            // Send request
            var content = new StringContent(encodedImage, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await _client.PostAsync(detectURL, content);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        catch (Exception ex)
        {
            logger.LogWarning("Detection failed");
            throw new ApplicationException($"Detection failed: {ex.Message}", ex);
        }
    }
}
