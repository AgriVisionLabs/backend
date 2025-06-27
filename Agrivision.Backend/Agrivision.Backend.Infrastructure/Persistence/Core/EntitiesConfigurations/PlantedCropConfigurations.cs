using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class PlantedCropConfigurations : IEntityTypeConfiguration<PlantedCrop>
{
    public void Configure(EntityTypeBuilder<PlantedCrop> builder)
    {
        builder.ToTable("PlantedCrops");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Id)
            .ValueGeneratedOnAdd();

        builder.Property(pc => pc.PlantingDate)
            .IsRequired();

        builder.Property(pc => pc.EstimatedYield)
            .HasPrecision(18, 2);

        builder.Property(pc => pc.ActualYield)
            .HasPrecision(18, 2);
        
        builder.HasOne(pc => pc.Crop)
            .WithMany(c => c.PlantedCrops)
            .HasForeignKey(pc => pc.CropId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pc => pc.Field)
            .WithOne(f => f.PlantedCrop)
            .HasForeignKey<PlantedCrop>(pc => pc.FieldId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}