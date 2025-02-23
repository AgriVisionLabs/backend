using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Commands;

public record RegisterCommand
(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber
) : IRequest<Result>; // IRequest<TResponse> is an interface from MediatR that represents a request that gets processed by a handler.
// Result is the return type (a wrapper for success/failure with optional data).
// It is part of the CQRS (Command Query Responsibility Segregation) pattern.