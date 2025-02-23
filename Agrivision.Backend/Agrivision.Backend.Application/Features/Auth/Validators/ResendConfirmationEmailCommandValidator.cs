using Agrivision.Backend.Application.Features.Auth.Commands;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmationEmailCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();
    }
}