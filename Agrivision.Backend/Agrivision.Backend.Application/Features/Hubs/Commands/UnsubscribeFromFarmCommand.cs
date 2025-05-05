using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Commands;

public record UnsubscribeFromFarmCommand(string ConnectionId) : IRequest<Unit>;