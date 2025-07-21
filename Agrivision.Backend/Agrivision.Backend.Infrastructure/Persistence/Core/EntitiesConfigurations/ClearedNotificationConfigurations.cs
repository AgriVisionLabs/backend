using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class ClearedNotificationConfigurations : IEntityTypeConfiguration<ClearedNotification>
{
    public void Configure(EntityTypeBuilder<ClearedNotification> builder)
    {
        builder.ToTable("ClearedNotifications");

        builder.HasKey(n => new { n.ClearedAt, n.UserId });

        builder.Property(n => n.UserId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(n => n.ClearedAt)
            .IsRequired();

        builder.HasIndex(n => n.UserId);
    }
}