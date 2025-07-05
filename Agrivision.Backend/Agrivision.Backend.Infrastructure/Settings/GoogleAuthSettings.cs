using System.ComponentModel.DataAnnotations;

namespace Agrivision.Backend.Infrastructure.Settings;

public class GoogleAuthSettings
{
    public static string SectionName { get; } = "GoogleAuthSettings";
    
    [Required]
    public string ClientId { get; set; } = string.Empty;
    [Required]
    public string ClientSecret { get; set; } = string.Empty;
} 