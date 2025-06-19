using Agrivision.Backend.Application.Features.Tasks.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Queries;

public record GetTaskItemByIdQuery
(
    Guid FarmId,
    string RequesterId,
    Guid TaskItemId
) : IRequest<Result<TaskItemResponse>>;