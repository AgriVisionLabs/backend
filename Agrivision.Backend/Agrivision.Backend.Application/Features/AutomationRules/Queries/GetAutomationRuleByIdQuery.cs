using Agrivision.Backend.Application.Features.AutomationRules.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Queries;

public record GetAutomationRuleByIdQuery
(
    Guid FarmId,
    Guid FieldId,
    Guid AutomationRuleId,
    string RequesterId
) : IRequest<Result<AutomationRuleResponse>>;