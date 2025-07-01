using System.ComponentModel.DataAnnotations;

namespace Agrivision.Backend.Application.Settings;

public class ServerSettings
{
    [Required]
    public static string SectionName { get; } = "ServerSettings";
    [Required]
    public string BaseUrl { get; set; } = string.Empty;
}