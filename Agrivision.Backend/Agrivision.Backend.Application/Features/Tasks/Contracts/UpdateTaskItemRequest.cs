using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Tasks.Contracts;

public record UpdateTaskItemRequest
(
    string Title,
    string? Description,
    string? AssignedToId,
    DateTime? DueDate,
    TaskItemPriority ItemPriority,
    TaskCategoryType Category
);