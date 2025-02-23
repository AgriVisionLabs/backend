using Agrivision.Backend.Application.Features.Auth.Contracts;
using Agrivision.Backend.Application.Features.Auth.Queries;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class AuthQueryValidator : AbstractValidator<AuthQuery>
{
    public AuthQueryValidator()
    {
        RuleFor(query => query.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(query => query.Password)
            .NotEmpty();
    }
}