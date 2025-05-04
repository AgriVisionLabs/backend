using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class SensorUnitConfigurations : IEntityTypeConfiguration<SensorUnit>
{
    public void Configure(EntityTypeBuilder<SensorUnit> builder)
    {
        builder.ToTable("SensorUnits");
        
        builder.HasKey(su => su.Id);
        
        builder.Property(su => su.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(su => su.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(su => su.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(su => new { su.FarmId, su.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        
        builder.Property(su => su.InstallationDate)
            .IsRequired();

        builder.Property(su => su.Status)
            .IsRequired();

        builder.Property(su => su.IpAddress)
            .HasMaxLength(100);

        builder.Property(su => su.Notes)
            .HasMaxLength(500);

        builder.Property(su => su.DeviceId)
            .IsRequired();
        
        builder.Property(su => su.ConfigJson)
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(su => su.DeviceId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

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
        
        builder.HasIndex(unit => unit.FarmId);
        
        builder.HasIndex(unit => unit.FieldId);
        
        builder.HasIndex(unit => unit.IsOnline);
        
    }
}