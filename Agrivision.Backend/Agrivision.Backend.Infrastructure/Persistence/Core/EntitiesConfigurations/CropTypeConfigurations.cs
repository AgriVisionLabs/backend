using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;
public class CropTypeConfigurations : IEntityTypeConfiguration<CropType>
{
    public void Configure(EntityTypeBuilder<CropType> builder)
    {
        builder.Property(C => C.Name)
           .IsRequired();
           

        builder.Property(C => C.SupportsDiseaseDetection)
           .IsRequired();

        builder.HasIndex(C => C.Name)
          .IsUnique();

    }
}
