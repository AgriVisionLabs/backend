using Agrivision.Backend.Application.Features.IrrigationEvents.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationEvents.Queries;

public record GetAllIrrigationEventsByFarmIdQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<List<IrrigationEventResponse>>>;