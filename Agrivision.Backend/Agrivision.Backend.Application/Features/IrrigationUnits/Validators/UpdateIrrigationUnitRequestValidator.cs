using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Validators;

public class UpdateIrrigationUnitRequestValidator : AbstractValidator<UpdateIrrigationUnitRequest>
{
    public UpdateIrrigationUnitRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(r => r.NewFieldId)
            .NotEmpty();

        RuleFor(r => r.Status)
            .NotNull()
            .IsInEnum();
    }
}