using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Validators;

public class AddIrrigationUnitRequestValidator : AbstractValidator<AddIrrigationUnitRequest>
{
    public AddIrrigationUnitRequestValidator()
    {
        RuleFor(u => u.SerialNumber)
            .NotEmpty()
            .Length(17);

        RuleFor(u => u.Name)
            .NotEmpty()
            .MinimumLength(3);
    }
}