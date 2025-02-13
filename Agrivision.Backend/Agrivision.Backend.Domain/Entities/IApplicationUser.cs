using agrivision_backend.Domain.Enums;

namespace Agrivision.Backend.Domain.Entities;

public interface IApplicationUser
{ 
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    // Security Features
    public DateTime? PasswordChangedAt { get; set; }
    public ApplicationUserStatus Status { get; set; }

    // Activity Tracking
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}