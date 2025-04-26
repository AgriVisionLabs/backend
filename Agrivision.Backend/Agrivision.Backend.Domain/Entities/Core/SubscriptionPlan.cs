
using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;
public class SubscriptionPlan:AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public decimal Price { get; set; } 
    public string Currency { get; set; } = "EGP"; 
    public int MaxFarms { get; set; } 
    public int MaxFields { get; set; } 
    public bool UnlimitedAiFeatureUsage { get; set; } 
    public bool IsActive { get; set; } = true;
}
