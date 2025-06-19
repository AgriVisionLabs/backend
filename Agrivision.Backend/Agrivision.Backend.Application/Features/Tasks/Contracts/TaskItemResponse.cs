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
    string? ClaimedById,
    string? ClaimedBy,
    string Title,
    string? Description,
    DateTime? DueDate,
    DateTime? CompletedAt,
    TaskItemPriority ItemPriority
);