using MediatR;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Application.Features.Account.Contracts;

namespace Agrivision.Backend.Application.Features.Account.Queries;
public record GetUserProfileQuery
    (
      string userId
    ) : IRequest<Result<UserProfileResponse>>;
