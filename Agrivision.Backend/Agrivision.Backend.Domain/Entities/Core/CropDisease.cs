using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;

public class CropDisease : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public List<string> Treatments { get; set; } = new List<string>();
    
    public Guid CropId { get; set; }
    public Crop Crop { get; set; } = default!;
}