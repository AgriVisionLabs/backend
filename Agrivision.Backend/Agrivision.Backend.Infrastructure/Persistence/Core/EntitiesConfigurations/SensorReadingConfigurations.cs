using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class SensorReadingConfigurations : IEntityTypeConfiguration<SensorReading>
{
    public void Configure(EntityTypeBuilder<SensorReading> builder)
    {
        builder.ToTable("SensorReadings");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.SensorConfigurationId)
            .IsRequired();

        builder.Property(r => r.TimeStamp)
            .IsRequired();

        builder.Property(r => r.Value)
            .IsRequired();

        builder.Property(r => r.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(r => r.SensorConfiguration)
            .WithMany(config => config.SensorReadings)
            .HasForeignKey(r => r.SensorConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}