using System.ComponentModel.DataAnnotations;

namespace Agrivision.Backend.Application.Settings;

public class AppSettings
{
    [Required]
    public static string SectionName { get; } = "AppSettings";
    [Required]
    public string BaseUrl { get; set; } = string.Empty;
    
}