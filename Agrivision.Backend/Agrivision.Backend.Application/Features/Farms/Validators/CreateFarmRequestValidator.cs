using Agrivision.Backend.Application.Features.Farms.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Farms.Validators;

public class CreateFarmRequestValidator : AbstractValidator<CreateFarmRequest>
{
    public CreateFarmRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(request => request.Area)
            .NotEmpty()
            .GreaterThanOrEqualTo(0.25)
            .WithMessage("The farm area must be at least 0.25 acres.");

        RuleFor(request => request.Location)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(request => request.SoilType)
            .IsInEnum();
    }
}