using Agrivision.Backend.Application.Features.Auth.Contracts;
using FluentValidation;


namespace Agrivision.Backend.Application.Features.Auth.Validators;
public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
