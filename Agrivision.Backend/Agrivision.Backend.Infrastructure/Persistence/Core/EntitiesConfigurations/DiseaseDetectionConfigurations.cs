using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;
public class DiseaseDetectionConfigurations : IEntityTypeConfiguration<DiseaseDetection>
{
    public void Configure(EntityTypeBuilder<DiseaseDetection> builder)
    {
        builder.Property(DD => DD.Status)
           .IsRequired();

        builder.Property(DD => DD.CreatedById)
           .IsRequired();

        builder.Property(DD => DD.CreatedOn)
           .IsRequired();

        
    }
}
