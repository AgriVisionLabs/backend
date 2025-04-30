using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Commands;

public record ToggleIrrigationUnitCommand
(
    Guid FarmId,
    Guid FieldId,
    string RequesterId,
    string RequesterName
) : IRequest<Result<ToggleIrrigationUnitResponse>>;