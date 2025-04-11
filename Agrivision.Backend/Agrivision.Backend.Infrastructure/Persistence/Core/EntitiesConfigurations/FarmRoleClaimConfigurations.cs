using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class FarmRoleClaimConfigurations : IEntityTypeConfiguration<FarmRoleClaim>
{
    public void Configure(EntityTypeBuilder<FarmRoleClaim> builder)
    {
        builder.ToTable("FarmRoleClaims");

        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.Id)
            .ValueGeneratedOnAdd();

        builder.Property(rc => rc.ClaimType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rc => rc.ClaimValue)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasOne(rc => rc.FarmRole)
            .WithMany(r => r.Claims)
            .HasForeignKey(rc => rc.FarmRoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(rc => new { rc.FarmRoleId, rc.ClaimType, rc.ClaimValue })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}