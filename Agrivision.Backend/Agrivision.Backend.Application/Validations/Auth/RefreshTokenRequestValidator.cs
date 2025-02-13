using Agrivision.Backend.Application.Contracts.Auth;
using FluentValidation;

namespace Agrivision.Backend.Application.Validations.Auth;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(request => request.Token)
            .NotEmpty();

        RuleFor(request => request.RefreshToken)
            .NotEmpty();
    }
}