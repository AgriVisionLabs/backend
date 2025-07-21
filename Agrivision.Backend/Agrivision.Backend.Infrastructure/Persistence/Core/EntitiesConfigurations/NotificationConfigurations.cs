using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class NotificationConfigurations : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);
        
        builder.Property(n => n.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.HasOne(n => n.Farm)
            .WithMany(f => f.Notifications)
            .HasForeignKey(n => n.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(n => n.Field)
            .WithMany(f => f.Notifications)
            .HasForeignKey(n => n.FieldId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}