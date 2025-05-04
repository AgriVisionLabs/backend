using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;
public class DiseaseDetection :AuditableEntity
{
    public Guid Id { get; set; }
    public DetecionResults Status { get; set; }
    
    public string ImagePath { get; set; }=string.Empty;

    ////// navigational property
    public Guid FarmId { get; set; }
    public Farm Farm { get; set; } = default!;
   
    public Guid FieldId { get; set; }
    public Field Field { get; set; } = default!;
  
    public Guid DiseaseId {  get; set; }
    public Disease Disease { get; set; }= default!;

}
