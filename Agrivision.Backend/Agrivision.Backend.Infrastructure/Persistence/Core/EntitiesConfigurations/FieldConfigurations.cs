using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class FieldConfigurations : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.ToTable("Fields");
        
        builder.HasKey(field => field.Id);
        
        builder.Property(field => field.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(field => field.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(field => field.Area)
            .IsRequired();

        builder.Property(field => field.IsActive)
            .IsRequired();

        builder.Property(field => field.CropTypeId)
           .IsRequired();

        builder.HasIndex(field => new { field.Name, field.FarmId })
            .IsUnique() // field name is unique per user
            .HasFilter("[IsDeleted] = 0");
        
        // could be omitted since we added practically the same thing in the farm configuration (used it here for a more clear code)
        builder.HasOne(field => field.Farm)
            .WithMany(farm => farm.Fields)
            .HasForeignKey(field => field.FarmId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(field => field.IrrigationUnits)
            .WithOne(iu => iu.Field)
            .HasForeignKey(iu => iu.FieldId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}