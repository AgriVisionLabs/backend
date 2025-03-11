using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class Farm : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Area { get; set; }
    public string Location { get; set; }
    public SoilTypes SoilType { get; set; }
    public List<FarmMember> FarmMembers { get; set; } = [];
}