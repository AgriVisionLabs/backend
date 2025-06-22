using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class TaskItem : AuditableEntity
{
    public Guid Id { get; set; }
    
    public string? AssignedToId { get; set; }
    public DateTime? AssignedAt { get; set; }
    public string? ClaimedById { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public TaskCategoryType Category { get; set; } = TaskCategoryType.Irrigation;
    
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    public TaskItemPriority ItemPriority { get; set; } = TaskItemPriority.Low;
    
    public Guid FieldId { get; set; }
    public Field Field { get; set; } = default!;
}