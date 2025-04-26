using System.ComponentModel.DataAnnotations;

namespace Agrivision.Backend.Infrastructure.Settings;
public class StripeSettings
{
    [Required]
    public string SecretKey { get; set; } = string.Empty;
    [Required]
    public string PublishableKey { get; set; } = string.Empty;
}
