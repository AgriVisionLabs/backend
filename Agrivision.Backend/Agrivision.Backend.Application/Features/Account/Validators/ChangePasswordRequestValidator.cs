using Agrivision.Backend.Application.Features.Account.Contracts;
using Agrivision.Backend.Domain.Abstractions.Consts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Account.Validators;
public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(request => request.CurrentPassword)
           .NotEmpty();

        RuleFor(request => request.NewPassword)
           .NotEmpty()
           .Matches(RegexPatterns.Password)
             .WithMessage("Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.")
           .NotEqual(request => request.CurrentPassword)
             .WithMessage("new password can not be as the current password.");


    }
}