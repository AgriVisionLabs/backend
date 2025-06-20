using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Commands;

public record UpdateTaskItemCommand
(
    Guid FarmId,
    Guid TaskItemId,
    string RequesterId,
    string Title,
    string? Description,
    string? AssignedToId,
    DateTime? DueDate,
    TaskItemPriority ItemPriority
) : IRequest<Result>;