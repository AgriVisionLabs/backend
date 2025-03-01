using Agrivision.Backend.Domain.Entities.Identity;
using Agrivision.Backend.Domain.Enums.Identity;
using Agrivision.Backend.Domain.Interfaces.Identity;

namespace Agrivision.Backend.Application.Models;

// this a class used to pass the data of something like a RegisterRequest to a function that expects IApplicationUser to then map it to ApplicationUser 
// you may ask why not just use RegisterRequest -> it is because then we won't be able to use that function for anything but a register request
public class ApplicationUserModel : IApplicationUser 
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public DateTime? PasswordChangedAt { get; set; }
    public ApplicationUserStatus Status { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; }
}