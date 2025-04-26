using Agrivision.Backend.Application.Features.IrrigationUnits.Commands;
using Agrivision.Backend.Application.Services.IoT;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Handlers;

public class ToggleIrrigationUnitCommandHandler(IWebSocketDeviceCommunicator communicator) : IRequestHandler<ToggleIrrigationUnitCommand, Result>
{
    public Task<Result> Handle(ToggleIrrigationUnitCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}