using Agrivision.Backend.Application.Features.Auth.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Commands;

public record RefreshTokenCommand
(
    string Token,
    string RefreshToken
) : IRequest<Result<AuthResponse>>;