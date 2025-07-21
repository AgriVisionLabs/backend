using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class ReadNotificationConfigurations : IEntityTypeConfiguration<ReadNotification>
{
    public void Configure(EntityTypeBuilder<ReadNotification> builder)
    {
        builder.ToTable("ReadNotifications");
        
        builder.HasKey(rn => new { rn.NotificationId, rn.UserId });
        
        builder.Property(rn => rn.NotificationId)
            .IsRequired();
        
        builder.Property(rn => rn.UserId)
            .IsRequired()
            .HasMaxLength(64);
        
        builder.Property(rn => rn.ReadAt)
            .IsRequired();

        builder.HasIndex(rn => rn.UserId);
        
        builder.HasOne(rn => rn.Notification)
            .WithMany(n => n.ReadNotifications)
            .HasForeignKey(rn => rn.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}