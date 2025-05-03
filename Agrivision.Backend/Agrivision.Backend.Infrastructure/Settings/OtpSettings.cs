using System.ComponentModel.DataAnnotations;
using Agrivision.Backend.Infrastructure.Config.Otp;

namespace Agrivision.Backend.Infrastructure.Settings;

public class OtpSettings
{
    public static string SectionName { get; } = "OtpSettings";

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public PerPurposeOtpConfig Verification { get; set; } = new();

    [Required]
    public PerPurposeOtpConfig PasswordReset { get; set; } = new();
    
    [Required]
    public PerPurposeOtpConfig TwoFactor { get; set; } = new();
}