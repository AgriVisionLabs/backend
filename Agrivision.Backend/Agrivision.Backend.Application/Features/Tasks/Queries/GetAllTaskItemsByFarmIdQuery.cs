using Agrivision.Backend.Application.Features.Tasks.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Queries;

public record GetAllTaskItemsByFarmIdQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<IReadOnlyList<TaskItemResponse>>>;