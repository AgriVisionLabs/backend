using Agrivision.Backend.Application.Features.Tasks.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Commands;

public record AddTaskItemCommand
(
    Guid FarmId,
    Guid FieldId,
    string RequesterId,
    string? AssignedToId,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskItemPriority ItemPriority
) : IRequest<Result<TaskItemResponse>>;