using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;
public class CropType : AuditableEntity
{
    public Guid Id { get; set; }
    public CropTypes Name { get; set; }   
    public bool SupportsDiseaseDetection { get; set; } // Whether we support disease detection for this crop


    public ICollection<Field> Fields { get; set; } = [];
    public ICollection<Disease> Diseases { get; set; } = [];
}
