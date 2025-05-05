
using System.ComponentModel.DataAnnotations;

namespace Agrivision.Backend.Infrastructure.Settings;
public class DetectionModelSettings
{
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string ModelId { get; set; } = string.Empty;

    [Required]
    public string BaseDetectionUrl { get; set; } = string.Empty;
}
