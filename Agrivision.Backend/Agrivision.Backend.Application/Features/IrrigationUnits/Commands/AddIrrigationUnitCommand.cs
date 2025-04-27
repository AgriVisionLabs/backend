using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Commands;

public record AddIrrigationUnitCommand
(
    Guid FarmId,
    Guid FieldId,
    string RequesterId,
    string RequesterName,
    string SerialNumber,
    string Name
) : IRequest<Result<IrrigationUnitResponse>>;