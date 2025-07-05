using System.ComponentModel.DataAnnotations;

namespace Agrivision.Backend.Infrastructure.Settings;

public class DiseaseDetectionSettings
{
    public static string SectionName { get; } = "DiseaseDetectionSettings";
    [Required]
    public string ImageDetectionModelToken { get; set; }
    [Required]
    public string VideoDetectionModelToken { get; set; }
    [Required]
    public string ImagePredictionUrl { get; set; }
    [Required]
    public string VideoPredictionUrl { get; set; }
}