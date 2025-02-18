using System.Runtime.InteropServices.JavaScript;
using agrivision_backend.Domain.Enums;
using Agrivision.Backend.Domain.Entities;

namespace Agrivision.Backend.Application.Models;

public class ApplicationUserModel : IApplicationUser
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? PasswordChangedAt { get; set; }
    public ApplicationUserStatus Status { get; set; } = ApplicationUserStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}