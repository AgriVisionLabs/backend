using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Domain.Abstractions.Consts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.UserName)
            .NotEmpty()
            .Length(3, 32);

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

        RuleFor(command => command.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(command => command.LastName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(command => command.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$") // E.164 international format
            .WithMessage("Invalid phone number format.");
    }
}