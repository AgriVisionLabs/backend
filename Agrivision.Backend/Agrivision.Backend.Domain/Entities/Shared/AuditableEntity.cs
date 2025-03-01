namespace Agrivision.Backend.Domain.Entities.Shared;

public class AuditableEntity
{
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
    
    public string CreatedById { get; set; }  // Only store user ID and not the navigational property since we have two databases
    public string? UpdatedById { get; set; }
}