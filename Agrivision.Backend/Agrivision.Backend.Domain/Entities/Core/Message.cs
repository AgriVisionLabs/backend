using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;

public class Message : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string SenderId { get; set; } = string.Empty;

    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
}