using Agrivision.Backend.Application.Features.SensorUnits.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.SensorUnits.Validators;

public class UpdateSensorUnitRequestValidator : AbstractValidator<UpdateSensorUnitRequest>
{
    public UpdateSensorUnitRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(r => r.NewFieldId)
            .NotEmpty();

        RuleFor(r => r.Status)
            .NotEmpty();
    }
}