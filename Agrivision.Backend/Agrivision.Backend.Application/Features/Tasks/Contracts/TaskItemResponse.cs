using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Tasks.Contracts;

public record TaskItemResponse
(
    Guid Id,
    Guid FarmId,
    Guid FieldId,
    string CreatedById,
    string CreatedBy,
    DateTime CreatedAt,
    string? AssignedToId,
    string? AssignedTo,
    DateTime? AssignedAt,
    string? ClaimedById,
    string? ClaimedBy,
    DateTime? ClaimedAt,
    string Title,
    string? Description,
    DateTime? DueDate,
    DateTime? CompletedAt,
    TaskItemPriority ItemPriority
);