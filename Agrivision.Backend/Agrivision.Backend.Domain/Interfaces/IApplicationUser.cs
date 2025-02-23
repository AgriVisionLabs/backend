using agrivision_backend.Domain.Enums;
using Agrivision.Backend.Domain.Entities;

namespace Agrivision.Backend.Domain.Interfaces;

public interface IApplicationUser
{ 
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
    
    // Security Features
    public DateTime? PasswordChangedAt { get; set; }
    public ApplicationUserStatus Status { get; set; }
    public bool EmailConfirmed { get;}

    // Activity Tracking
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<RefreshToken> RefreshTokens { get; set; }
}