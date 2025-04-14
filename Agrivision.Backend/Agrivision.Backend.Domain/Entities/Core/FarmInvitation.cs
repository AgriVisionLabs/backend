using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;

public class FarmInvitation : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid FarmId { get; set; }
    public int FarmRoleId { get; set; }
    public string Token { get; set; }
    public string InvitedEmail { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public Farm Farm { get; set; } = default!;
    public FarmRole FarmRole { get; set; } = default!;
}