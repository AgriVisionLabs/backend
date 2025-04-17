using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Features.Auth.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;
public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(req => req.Token)
            .NotEmpty();
    }
}