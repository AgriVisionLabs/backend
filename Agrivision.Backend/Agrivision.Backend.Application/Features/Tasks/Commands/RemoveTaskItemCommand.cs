using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Tasks.Commands;

public record RemoveTaskItemCommand
(
    Guid FarmId,
    Guid TaskItemId,
    string RequesterId
) : IRequest<Result>;
