using System.ComponentModel.DataAnnotations;

namespace Agrivision.Backend.Application.Settings;

public class RefreshTokenSettings
{
    [Required]
    public static string SectionName { get; } = "RefreshTokenSettings";
    [Range(1, int.MaxValue)]
    public int RefreshTokenExpiryDays { get; set; }
}