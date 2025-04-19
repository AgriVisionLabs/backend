using Agrivision.Backend.Application.Features.Field.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Field.Queries;

public record GetAllFieldsByFarmIdQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<List<FieldResponse>>>;