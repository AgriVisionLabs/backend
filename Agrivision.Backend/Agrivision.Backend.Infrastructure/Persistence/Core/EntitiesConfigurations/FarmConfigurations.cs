using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class FarmConfigurations : IEntityTypeConfiguration<Farm>
{
    public void Configure(EntityTypeBuilder<Farm> builder)
    {
        builder.ToTable("Farms");

        builder.HasKey(farm => farm.Id);

        builder.Property(farm => farm.Id)
            .ValueGeneratedOnAdd();

        builder.Property(farm => farm.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(farm => farm.Area)
            .IsRequired();

        builder.Property(farm => farm.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(farm => farm.SoilType)
            .IsRequired();

        builder.HasIndex(farm => new { farm.Name, farm.CreatedById }).IsUnique();


        builder.OwnsMany(x => x.FarmMembers)
              .ToTable("farmMembers")
              .WithOwner()
              .HasForeignKey("UserId");
    }
}