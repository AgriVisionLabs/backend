using Agrivision.Backend.Application.Features.Fields.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Queries;

public record GetAllFieldsByFarmIdQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<List<FieldResponse>>>;