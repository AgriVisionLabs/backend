
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Commands;
public record ResetPasswordCommand
(
   string Email,
   string Otp,
   string NewPassword
):IRequest<Result>;
