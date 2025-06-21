using System.ComponentModel.DataAnnotations;

namespace Agrivision.Backend.Infrastructure.Settings;

public class MailSettings
{ 
    public static string SectionName { get; } = "MailSettings";
    [Required]
    public string SmtpServer { get; set; } = string.Empty;
    [Required]
    public int Port { get; set; }
    [Required]
    public string SenderEmail { get; set; } = string.Empty;
    [Required]
    public string SenderName { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public bool EnableSSL = false;
}