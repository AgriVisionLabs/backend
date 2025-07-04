using System.ComponentModel.DataAnnotations;

namespace Agrivision.Backend.Infrastructure.Settings;
public class StripeSettings
{
    [Required]
    public string SecretKey { get; set; } = string.Empty;
    [Required]
    public string PublishableKey { get; set; } = string.Empty;
    [Required]
    public string SuccessUrl { get; set; } = "https://localhost:7193/Subscriptions/confirm-subscription";
    [Required]
    public string CancelUrl { get; set; } = "https://localhost:7193/Subscriptions/??????????";
    [Required]
    public string WebhookSecret { get; set; } = "";


}
