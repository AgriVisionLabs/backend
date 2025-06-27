using Agrivision.Backend.Application.Features.Fields.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Fields.Validators;

public class UpdateFarmRequestValidator : AbstractValidator<UpdateFieldRequest>
{
    public UpdateFarmRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(request => request.Area)
            .NotEmpty()
            .GreaterThanOrEqualTo(0.25)
            .WithMessage("The farm area must be at least 0.25 acres.");
    }
}