using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Commands;

public record RemoveAutomationRuleCommand
(
    Guid FarmId,
    Guid FieldId,
    Guid AutomationRuleId,
    string RequesterId
) : IRequest<Result>;