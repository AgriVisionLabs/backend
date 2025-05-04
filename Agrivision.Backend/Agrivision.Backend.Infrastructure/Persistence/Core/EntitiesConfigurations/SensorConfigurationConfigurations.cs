using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class SensorConfigurationConfigurations : IEntityTypeConfiguration<SensorConfiguration>
{
    public void Configure(EntityTypeBuilder<SensorConfiguration> builder)
    {
        builder.ToTable("SensorConfigurations");

        builder.HasKey(config => config.Id);

        builder.Property(config => config.Id)
            .ValueGeneratedNever();

        builder.Property(config => config.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(config => config.Pin)
            .IsRequired()
            .HasMaxLength(100); // Adjust if you're into long, expressive pin names like "GPIO_23_DHT11"

        builder.Property(config => config.CalibrationDataJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(config => config.IsActive)
            .IsRequired();

        builder.HasOne(config => config.SensorUnitDevice)
            .WithMany(device => device.SensorConfigurations)
            .HasForeignKey(config => config.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(config => config.SensorReadings)
            .WithOne(reading => reading.SensorConfiguration)
            .HasForeignKey(reading => reading.SensorConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(config => config.DeviceId);

        builder.HasIndex(config => config.Type);
        
        builder.HasIndex(config => config.IsActive);
    }
}