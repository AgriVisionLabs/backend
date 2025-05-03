using Agrivision.Backend.Application.Features.Auth.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class RequestPasswordResetRequestValidator : AbstractValidator<RequestPasswordResetRequest>
{
    public RequestPasswordResetRequestValidator()
    {
        RuleFor(req => req.Email)
            .EmailAddress()
            .NotEmpty();
    }
}