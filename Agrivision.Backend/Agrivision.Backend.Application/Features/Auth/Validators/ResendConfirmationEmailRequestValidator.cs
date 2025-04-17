using Agrivision.Backend.Application.Features.Auth.Commands;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(req => req.Email)
            .NotEmpty()
            .EmailAddress();
    }
}