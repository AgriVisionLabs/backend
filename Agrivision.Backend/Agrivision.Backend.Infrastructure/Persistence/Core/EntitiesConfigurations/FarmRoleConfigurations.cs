using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class FarmRoleConfigurations : IEntityTypeConfiguration<FarmRole>
{
    public void Configure(EntityTypeBuilder<FarmRole> builder)
    {
        builder.ToTable("FarmRoles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(250);

        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasMany(r => r.FarmUserRoles)
            .WithOne(fur => fur.FarmRole)
            .HasForeignKey(fur => fur.FarmRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Claims)
            .WithOne(rc => rc.FarmRole)
            .HasForeignKey(rc => rc.FarmRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}