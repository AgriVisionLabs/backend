using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class IrrigationUnitConfigurations : IEntityTypeConfiguration<IrrigationUnit>
{
    public void Configure(EntityTypeBuilder<IrrigationUnit> builder)
    {
        builder.ToTable("IrrigationUnits");
        
        builder.HasKey(iu => iu.Id);
        
        builder.Property(iu => iu.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(iu => iu.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(iu => new { iu.FarmId, iu.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        
        builder.Property(iu => iu.InstallationDate)
            .IsRequired();

        builder.Property(iu => iu.Status)
            .IsRequired();

        builder.Property(iu => iu.IpAddress)
            .HasMaxLength(100);

        builder.Property(iu => iu.Notes)
            .HasMaxLength(500);

        builder.Property(iu => iu.DeviceId)
            .IsRequired();
        
        builder.Property(iu => iu.ConfigJson)
            .HasColumnType("nvarchar(max)");
        
        builder.HasOne(iu => iu.Device)
            .WithOne()
            .HasForeignKey<IrrigationUnit>(iu => iu.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(iu => iu.Farm)
            .WithMany(f => f.IrrigationUnits)
            .HasForeignKey(iu => iu.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(iu => iu.Field)
            .WithMany(f => f.IrrigationUnits)
            .HasForeignKey(iu => iu.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(iu => iu.DeviceId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        
        builder.HasIndex(iu => iu.FieldId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}