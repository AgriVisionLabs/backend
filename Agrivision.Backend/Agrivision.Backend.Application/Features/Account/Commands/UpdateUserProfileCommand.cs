using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Account.Commands;
public record UpdateUserProfileCommand
(
    string UserId,
    string FirstName,
    string LastName,
    string UserName,
    string? PhoneNumber
):IRequest<Result>;
