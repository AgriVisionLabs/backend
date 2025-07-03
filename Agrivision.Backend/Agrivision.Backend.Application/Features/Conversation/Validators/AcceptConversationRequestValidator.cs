using Agrivision.Backend.Application.Features.Conversation.Contracts;
using FluentValidation;

namespace Agrivision.Backend.Application.Features.Conversation.Validators;

public class AcceptConversationRequestValidator : AbstractValidator<AcceptConversationRequest>
{
    public AcceptConversationRequestValidator()
    {
        RuleFor(req => req.Accept)
            .NotNull();

        RuleFor(req => req.ConnectionId)
            .NotEmpty();
    }
}