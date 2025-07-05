namespace Agrivision.Backend.Application.Models;

public class GoogleUserPayload
{
    public string Email { get; set; } = string.Empty;
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public string Subject { get; set; } = string.Empty;
} 