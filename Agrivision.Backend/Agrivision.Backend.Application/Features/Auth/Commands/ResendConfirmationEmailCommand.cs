using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Commands;

public record ResendConfirmationEmailCommand
(
    string Email    
) : IRequest<Result>;