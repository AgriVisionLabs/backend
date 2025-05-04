using Agrivision.Backend.Application.Features.Fields.Contracts;
using Agrivision.Backend.Domain.Enums.Core;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Fields.Validators;

public class CreateFieldRequestValidator : AbstractValidator<CreateFieldRequest>
{
    public CreateFieldRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(request => request.Crop)
             .NotNull()
             .IsInEnum();

        RuleFor(request => request.Area)
            .NotEmpty()
            .GreaterThanOrEqualTo(0.25)
            .WithMessage("The farm area must be at least 0.25 acres.");
    }
}