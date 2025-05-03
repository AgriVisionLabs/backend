using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Commands;

public record VerifyPasswordResetOtpCommand
(
    string Email, 
    string OtpCode
) : IRequest<Result<string>>;