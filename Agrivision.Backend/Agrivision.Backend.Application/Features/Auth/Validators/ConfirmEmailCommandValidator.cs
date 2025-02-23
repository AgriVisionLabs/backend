using Agrivision.Backend.Application.Features.Auth.Commands;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;
public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(command => command.Token)
            .NotEmpty();
    }
}