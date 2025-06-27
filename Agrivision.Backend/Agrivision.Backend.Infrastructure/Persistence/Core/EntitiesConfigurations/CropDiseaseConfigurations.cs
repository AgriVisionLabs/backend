using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class CropDiseaseConfigurations : IEntityTypeConfiguration<CropDisease>
{
    public void Configure(EntityTypeBuilder<CropDisease> builder)
    {
        builder.ToTable("CropDiseases");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Treatments)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
            )
            .HasMaxLength(1000)
            .Metadata.SetValueComparer(
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                )
            );

        builder.HasOne(d => d.Crop)
            .WithMany(c => c.CropDiseases)
            .HasForeignKey(d => d.CropId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}