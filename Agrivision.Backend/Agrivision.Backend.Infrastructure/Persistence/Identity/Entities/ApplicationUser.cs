using agrivision_backend.Domain.Enums;
using Agrivision.Backend.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;

public class ApplicationUser : IdentityUser, IApplicationUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
    public ApplicationUserStatus Status { get; set; } = ApplicationUserStatus.Active;
    public bool EmailConfirmed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}