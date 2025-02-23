using Agrivision.Backend.Application.Features.Auth.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Queries;

public record AuthQuery
(
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;