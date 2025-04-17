using Agrivision.Backend.Domain.Abstractions;
using MediatR;


namespace Agrivision.Backend.Application.Features.Auth.Commands;
public record VerifyOtpCommand
(
    string Otp,
    string Email
):IRequest<Result>;