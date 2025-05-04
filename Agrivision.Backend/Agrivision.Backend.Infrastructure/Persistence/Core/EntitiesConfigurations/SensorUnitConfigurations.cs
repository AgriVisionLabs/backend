using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class SensorUnitConfigurations : IEntityTypeConfiguration<SensorUnit>
{
    public void Configure(EntityTypeBuilder<SensorUnit> builder)
    {
        builder.ToTable("SensorUnits");

        builder.HasKey(unit => unit.Id);

        builder.Property(unit => unit.Id)
            .ValueGeneratedNever(); 

        builder.Property(unit => unit.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(unit => unit.InstallationDate)
            .IsRequired();

        builder.Property(unit => unit.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(unit => unit.ConfigJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(unit => unit.IpAddress)
            .HasMaxLength(100);

        builder.Property(unit => unit.Notes)
            .HasMaxLength(500);

        builder.Property(unit => unit.CreatedBy)
            .IsRequired()
            .HasMaxLength(100); 

        builder.HasOne(unit => unit.Device)
            .WithOne()
            .HasForeignKey<SensorUnit>(unit => unit.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(unit => unit.Farm)
            .WithMany(farm => farm.SensorUnits)
            .HasForeignKey(unit => unit.FarmId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(unit => unit.Field)
            .WithMany(field => field.SensorUnits)
            .HasForeignKey(unit => unit.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(unit => unit.SensorConfigurations)
            .WithOne(config => config.SensorUnit)
            .HasForeignKey(config => config.SensorUnitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}