using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;


namespace Agrivision.Backend.Application.Features.Farm.Queries;

public record GetAllFarmsCreatedByUserIdQuery(string UserId)  : IRequest<Result<List<FarmResponse>>>;