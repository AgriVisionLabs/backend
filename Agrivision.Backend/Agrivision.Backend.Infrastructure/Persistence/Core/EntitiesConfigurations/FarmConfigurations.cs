using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class FarmConfigurations : IEntityTypeConfiguration<Farm>
{
    public void Configure(EntityTypeBuilder<Farm> builder)
    {
        builder.ToTable("Farms");

        builder.HasKey(farm => farm.Id);

        builder.Property(farm => farm.Id)
            .ValueGeneratedOnAdd();

        builder.Property(farm => farm.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(farm => farm.Area)
            .IsRequired();

        builder.Property(farm => farm.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(farm => farm.SoilType)
            .IsRequired();
        
        builder.Property(farm => farm.FieldsNo)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(farm => new { farm.Name, farm.CreatedById })
            .IsUnique() // farm name is unique per user
            .HasFilter("[IsDeleted] = 0");
        
        builder.HasMany(farm => farm.Fields) // each farm has many fields
            .WithOne(field => field.Farm) // each field has one farm 
            .HasForeignKey(field => field.FarmId) // farmId is the foreign key
            .OnDelete(DeleteBehavior.Restrict); // restrict deletion of fields when farms is deleted ig

        builder.HasMany(farm => farm.FarmUserRoles)
            .WithOne(fur => fur.Farm)
            .HasForeignKey(fur => fur.FarmId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(farm => farm.FarmInvitations)
            .WithOne(inv => inv.Farm)
            .HasForeignKey(inv => inv.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(farm => farm.IrrigationUnits)
            .WithOne(iu => iu.Farm)
            .HasForeignKey(iu => iu.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}