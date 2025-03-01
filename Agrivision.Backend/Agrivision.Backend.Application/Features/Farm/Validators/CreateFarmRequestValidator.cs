using Agrivision.Backend.Application.Features.Farm.Commands;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Farm.Validators;

public class CreateFarmCommandValidator : AbstractValidator<CreateFarmCommand>
{
    public CreateFarmCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(command => command.Area)
            .NotEmpty()
            .GreaterThanOrEqualTo(0.25)
            .WithMessage("The farm area must be at least 0.25 acres.");

        RuleFor(command => command.Location)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(command => command.SoilType)
            .NotEmpty();
    }
}