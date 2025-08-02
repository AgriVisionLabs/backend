using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Account.Commands;

public record UpdateMfaCommand(string UserId, bool IsEnabled) : IRequest<Result>;