using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class DiseaseDetectionConfigurations : IEntityTypeConfiguration<DiseaseDetection>
{
    public void Configure(EntityTypeBuilder<DiseaseDetection> builder)
    {
        builder.ToTable("DiseaseDetections");

        builder.HasIndex(dd => dd.Id);
        
        builder.Property(dd => dd.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(dd => dd.ConfidenceLevel)
            .IsRequired();

        builder.Property(dd => dd.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(dd => dd.PlantedCrop)
            .WithMany(pc => pc.DiseaseDetections)
            .HasForeignKey(dd => dd.PlantedCropId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dd => dd.CropDisease)
            .WithMany()
            .HasForeignKey(dd => dd.CropDiseaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}