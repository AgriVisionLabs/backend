using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;
public class DiseaseConfigurations : IEntityTypeConfiguration<Disease>
{
    public void Configure(EntityTypeBuilder<Disease> builder)
    {
        builder.ToTable("Diseases");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(d => d.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(d => d.ClassIdInModelPredictions)
            .IsRequired();

        builder.HasIndex(d => d.ClassIdInModelPredictions)
            .IsUnique();

        builder.Property(d => d.Is_Safe)
            .IsRequired();

        builder.HasOne(d => d.CropType)
            .WithMany(c => c.Diseases)
            .HasForeignKey(d => d.CropTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}