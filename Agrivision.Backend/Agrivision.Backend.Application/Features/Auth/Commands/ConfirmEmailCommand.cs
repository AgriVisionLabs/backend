using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Commands;

public record ConfirmEmailCommand
(
    string UserId,
    string Code
) : IRequest<Result>;