using Agrivision.Backend.Application.Features.Tasks.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Queries;

public record GetAllTaskItemsByFieldIdQuery
(
    Guid FarmId,
    Guid FieldId,
    string RequesterId
) : IRequest<Result<IReadOnlyList<TaskItemResponse>>>;