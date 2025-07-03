using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IClearedConversationRepository
{
    Task<ClearedConversation?> FindByUserAndConversation(string userId, Guid conversationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetClearedConversationIdsForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsClearedAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default);
    Task AddAsync(ClearedConversation entry, CancellationToken cancellationToken = default);
    Task RemoveAsync(ClearedConversation entry, CancellationToken cancellationToken = default);
}