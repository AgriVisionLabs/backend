using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Commands;

public record SubscribeToFarmCommand
(
    Guid FarmId,
    string ConnectionId,
    string UserId
) : IRequest<Result>;