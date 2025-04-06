namespace Agrivision.Backend.Domain.Entities.Core;

public class FarmRole
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<FarmUserRole> FarmUserRoles { get; set; } = new List<FarmUserRole>();
}