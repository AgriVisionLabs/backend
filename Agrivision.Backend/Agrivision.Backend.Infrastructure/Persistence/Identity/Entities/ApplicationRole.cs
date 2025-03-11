using Microsoft.AspNetCore.Identity;


namespace Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
public class ApplicationRole:IdentityRole
{
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
}
