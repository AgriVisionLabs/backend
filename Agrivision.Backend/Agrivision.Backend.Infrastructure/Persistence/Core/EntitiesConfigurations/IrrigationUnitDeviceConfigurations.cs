using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class IrrigationUnitDeviceConfigurations : IEntityTypeConfiguration<IrrigationUnitDevice>
{
    public void Configure(EntityTypeBuilder<IrrigationUnitDevice> builder)
    {
        builder.ToTable("IrrigationUnitDevices");

        builder.HasKey(device => device.Id);

        builder.Property(device => device.Id)
            .ValueGeneratedOnAdd();

        builder.Property(device => device.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(device => device.SerialNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(device => device.MacAddress)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(device => device.MacAddress)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(device => device.FirmwareVersion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(device => device.ManufacturedOn)
            .IsRequired();

        builder.Property(device => device.IsAssigned)
            .IsRequired();

        builder.Property(device => device.ProvisioningKey)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Used to verify secure ownership during manual provisioning.");
        
        builder.HasIndex(device => device.ProvisioningKey)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}