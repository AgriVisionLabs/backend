namespace Agrivision.Backend.Domain.Entities.Core;

public class FarmRoleClaim
{
    public int Id { get; set; }
    public int FarmRoleId { get; set; }
    public string ClaimType { get; set; } = default!;
    public string ClaimValue { get; set; } = default!;
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? DeletedById { get; set; }

    public FarmRole FarmRole { get; set; } = default!;
}