using Agrivision.Backend.Domain.Abstractions;
using MediatR;


namespace Agrivision.Backend.Application.Features.Account.Commands;
public record ChangePasswordCommand
(
     string UserId,
     string CurrentPassword,
     string NewPassword
): IRequest<Result>;
