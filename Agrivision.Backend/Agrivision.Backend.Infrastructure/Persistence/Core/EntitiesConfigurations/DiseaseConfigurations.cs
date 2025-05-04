using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;
public class DiseaseConfigurations : IEntityTypeConfiguration<Disease>
{
    public void Configure(EntityTypeBuilder<Disease> builder)
    {
        builder.Property(D => D.Name)
           .IsRequired();

        builder.Property(D => D.ClassIdInModelPredictions)
           .IsRequired();

        builder.Property(D => D.CropTypeId)
           .IsRequired();

        builder.HasIndex(D => D.ClassIdInModelPredictions)
           .IsUnique(); 

    }
}