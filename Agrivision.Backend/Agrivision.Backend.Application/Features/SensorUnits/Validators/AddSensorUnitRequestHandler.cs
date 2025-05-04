using Agrivision.Backend.Application.Features.SensorUnits.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.SensorUnits.Validators;

public class AddSensorUnitRequestHandler : AbstractValidator<AddSensorUnitRequest>
{
    public AddSensorUnitRequestHandler()
    {
        RuleFor(u => u.SerialNumber)
            .NotEmpty()
            .Length(15);

        RuleFor(u => u.Name)
            .NotEmpty()
            .Length(3, 100);
    }
}