using Agrivision.Backend.Application.Features.Account.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Account.Validators;

public class UpdateMfaRequestValidator : AbstractValidator<UpdateMfaRequest>
{
    public UpdateMfaRequestValidator()
    {
        RuleFor(r => r.IsEnabled)
            .NotNull();
    }
}