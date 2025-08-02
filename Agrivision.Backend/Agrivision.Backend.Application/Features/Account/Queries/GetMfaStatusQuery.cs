using Agrivision.Backend.Application.Features.Account.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Account.Queries;

public record GetMfaStatusQuery(string UserId) : IRequest<Result<MfaStatusResponse>>;