using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Commands;

public record ResetPasswordCommand
(
    string Token,
    string NewPassword
) : IRequest<Result>;