using Agrivision.Backend.Domain.Entities.Identity;
using Agrivision.Backend.Domain.Enums.Identity;
using Agrivision.Backend.Domain.Interfaces.Identity;
using Microsoft.AspNetCore.Identity;

namespace Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;

public class ApplicationUser : IdentityUser, IApplicationUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PfpImageUrl { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
    public ApplicationUserStatus Status { get; set; } = ApplicationUserStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}