using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;

public class FarmUserRole : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }
    public string UserId { get; set; }
    public int FarmRoleId { get; set; }

    public bool IsActive { get; set; } = false; // since it is invitation based so it is false until they accept 
    public DateTime? AcceptedAt { get; set; }

    public Farm Farm { get; set; } = default!;
    public FarmRole FarmRole { get; set; } = default!;
}