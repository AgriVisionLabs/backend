using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class MessageRepository(CoreDbContext coreDbContext) : IMessageRepository
{
    public async Task<Message?> FindByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Messages
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);
    }

    public async Task<IReadOnlyList<Message>> GetMessagesForConversationAsync(Guid conversationId, int take = 50, int skip = 0,
        CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedOn)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

    }

    public async Task<IReadOnlyList<Message>> GetVisibleMessagesForUserAsync(string userId, Guid conversationId, int take = 50, int skip = 0,
        CancellationToken cancellationToken = default)
    {
        // get when the user last cleared this convo
        var clearedAt = await coreDbContext.ClearedConversations
            .Where(c => c.UserId == userId && c.ConversationId == conversationId)
            .Select(c => (DateTime?)c.ClearedAt)
            .FirstOrDefaultAsync(cancellationToken);

        // build the base query
        var query = coreDbContext.Messages
            .Where(m => m.ConversationId == conversationId);

        if (clearedAt is not null)
        {
            query = query.Where(m => m.CreatedOn > clearedAt);
        }
        
        return await query
            .OrderByDescending(m => m.CreatedOn)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await coreDbContext.Messages.AddAsync(message, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Message message, CancellationToken cancellationToken = default)
    {
        coreDbContext.Messages.Update(message);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Message message, CancellationToken cancellationToken = default)
    {
        coreDbContext.Messages.Remove(message);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}