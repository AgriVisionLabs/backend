using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IAutomationRuleRepository
{
    // get by farmId 
    Task<IReadOnlyList<AutomationRule>> FindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);

    // get by id
    Task<AutomationRule?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    // add
    Task AddAsync(AutomationRule rule, CancellationToken cancellationToken = default);
    
    // update
    Task UpdateAsync(AutomationRule rule, CancellationToken cancellationToken = default);
    
    // find by name and farm (since name is unique per farm)
    Task<AutomationRule?> FindByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default);
}