using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Tasks.Contracts;

public record AddTaskItemRequest
(
    string? AssignedToId,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskItemPriority ItemPriority
);