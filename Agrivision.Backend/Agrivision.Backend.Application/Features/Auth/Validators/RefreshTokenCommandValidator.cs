using Agrivision.Backend.Application.Features.Auth.Commands;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Auth.Validators;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(req => req.Token)
            .NotEmpty();

        RuleFor(req => req.RefreshToken)
            .NotEmpty();
    }
}