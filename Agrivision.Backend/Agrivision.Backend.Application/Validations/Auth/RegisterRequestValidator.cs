using Agrivision.Backend.Application.Contracts.Auth;
using Agrivision.Backend.Domain.Abstractions.Consts;
using FluentValidation;

namespace Agrivision.Backend.Application.Validations.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.UserName)
            .NotEmpty()
            .Length(3, 32);

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(request => request.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

        RuleFor(request => request.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .Length(3, 100);
    }
}