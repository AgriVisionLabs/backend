using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Commands;

public record UpdateIrrigationUnitCommand
(
    Guid FarmId,
    Guid FieldId,
    string RequesterId,
    string Name,
    UnitStatus Status,
    Guid NewFieldId
) : IRequest<Result>;
