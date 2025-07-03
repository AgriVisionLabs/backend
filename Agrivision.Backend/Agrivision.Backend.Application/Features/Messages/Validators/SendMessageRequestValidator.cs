using Agrivision.Backend.Application.Features.Messages.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Messages.Validators;

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(req => req.Content)
            .NotEmpty()
            .MaximumLength(5000);
    }
}