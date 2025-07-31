using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.IrrigationEvents.Contracts;

public record IrrigationEventResponse
(
    Guid Id,
    Guid FarmId,
    Guid FieldId,
    string FieldName,
    string CropName,
    Guid IrrigationUnitId,
    DateTime StartTime,
    DateTime? EndTime,
    IrrigationTriggerMethod TriggerMethod
);