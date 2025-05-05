using Agrivision.Backend.Application.Features.AutomationRules.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Queries;

public record GetAutomationRulesByFarmIdQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<IReadOnlyList<AutomationRuleResponse>>>;