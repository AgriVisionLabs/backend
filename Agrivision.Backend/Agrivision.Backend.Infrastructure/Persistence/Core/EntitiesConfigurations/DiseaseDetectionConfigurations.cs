using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;
public class DiseaseDetectionConfigurations : IEntityTypeConfiguration<DiseaseDetection>
{
    public void Configure(EntityTypeBuilder<DiseaseDetection> builder)
    {
        builder.ToTable("DiseaseDetections");

        builder.HasKey(dd => dd.Id);

        builder.Property(dd => dd.Id)
            .ValueGeneratedOnAdd();

        builder.Property(dd => dd.Status)
            .IsRequired();

        builder.Property(dd => dd.ImagePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(dd => dd.Farm)
            .WithMany(f => f.DiseaseDetections)
            .HasForeignKey(dd => dd.FarmId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dd => dd.Field)
            .WithMany(f => f.DiseaseDetections)
            .HasForeignKey(dd => dd.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dd => dd.Disease)
            .WithMany(d => d.DiseaseDetections)
            .HasForeignKey(dd => dd.DiseaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
