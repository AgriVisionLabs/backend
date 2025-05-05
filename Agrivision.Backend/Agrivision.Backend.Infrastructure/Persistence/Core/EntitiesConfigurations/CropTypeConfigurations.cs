using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;
public class CropTypeConfigurations : IEntityTypeConfiguration<CropType>
{
    public void Configure(EntityTypeBuilder<CropType> builder)
    {
        builder.ToTable("CropTypes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd(); 

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(c => c.SupportsDiseaseDetection)
            .IsRequired();

        builder.HasMany(c => c.Fields)
            .WithOne(f => f.CropType)
            .HasForeignKey(f => f.CropTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Diseases)
            .WithOne(d => d.CropType)
            .HasForeignKey(d => d.CropTypeId)
            .OnDelete(DeleteBehavior.Cascade); 

    }
}
