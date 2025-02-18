using Agrivision.Backend.Application.Contracts.Auth;
using FluentValidation;

namespace Agrivision.Backend.Application.Validations.Auth;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(request => request.UserId)
            .NotEmpty();
        
        RuleFor(request => request.Code)
            .NotEmpty();

        
    }
}