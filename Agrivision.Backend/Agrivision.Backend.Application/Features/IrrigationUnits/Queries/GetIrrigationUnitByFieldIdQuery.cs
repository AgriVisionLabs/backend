using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Queries;

public record GetIrrigationUnitByFieldIdQuery
(
    Guid FarmId,
    Guid FieldId,
    string RequesterId,
    string RequesterName
) : IRequest<Result<IrrigationUnitResponse>>;

