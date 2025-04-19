

using Agrivision.Backend.Application.Features.Account.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Account.Validators;
public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(req => req.UserName)
            .NotEmpty()
            .Length(3, 32);

       
        RuleFor(req => req.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(req => req.LastName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(req => req.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$") // E.164 international format
            .WithMessage("Invalid phone number format.");
    }
}
