using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class NotificationPreferenceConfigurations : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("NotificationPreferences");
        
        builder.HasKey(np => new { np.UserId, np.NotificationType });
        
        builder.Property(np => np.UserId)
            .IsRequired()
            .HasMaxLength(64);
        
        builder.Property(np => np.NotificationType)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(np => np.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(np => np.UserId);
        
        builder.HasIndex(np => np.NotificationType)
            .HasDatabaseName("IX_NotificationPreferences_NotificationType");
    }
}