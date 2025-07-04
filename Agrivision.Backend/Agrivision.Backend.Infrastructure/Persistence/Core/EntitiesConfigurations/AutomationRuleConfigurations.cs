using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class AutomationRuleConfigurations : IEntityTypeConfiguration<AutomationRule>
{
    public void Configure(EntityTypeBuilder<AutomationRule> builder)
    {
        
        builder.ToTable("AutomationRules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.TargetSensorType)
            .HasConversion<string>();

        builder.Property(x => x.MinimumThresholdValue);
        
        builder.Property(x => x.MaximumThresholdValue);

        builder.Property(x => x.StartTime);

        builder.Property(x => x.EndTime);

        builder.Property(x => x.ActiveDays)
            .HasConversion<int>(); 

        builder.HasOne(x => x.SensorUnit)
            .WithMany() 
            .HasForeignKey(x => x.SensorUnitId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(x => x.IrrigationUnit)
            .WithMany(iu => iu.AutomationRules)
            .HasForeignKey(x => x.IrrigationUnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Farm)
            .WithMany(f => f.AutomationRules)
            .HasForeignKey(x => x.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.SensorUnitId);
        
        builder.HasIndex(x => x.IrrigationUnitId);

        builder.HasIndex(x => new { x.FarmId, x.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}