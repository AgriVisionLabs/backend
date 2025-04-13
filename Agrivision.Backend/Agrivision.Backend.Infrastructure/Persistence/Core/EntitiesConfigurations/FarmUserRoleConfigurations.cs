using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class FarmUserRoleConfigurations : IEntityTypeConfiguration<FarmUserRole>
{
    public void Configure(EntityTypeBuilder<FarmUserRole> builder)
    {
        builder.ToTable("FarmUserRoles");

        builder.HasKey(fur => fur.Id);

        builder.Property(fur => fur.UserId)
            .IsRequired();

        builder.Property(fur => fur.IsActive)
            .IsRequired();

        builder.Property(fur => fur.AcceptedAt)
            .IsRequired(false); // nullable until they accept 

        builder.HasOne(fur => fur.Farm)
            .WithMany(f => f.FarmUserRoles)
            .HasForeignKey(fur => fur.FarmId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fur => fur.FarmRole)
            .WithMany(r => r.FarmUserRoles)
            .HasForeignKey(fur => fur.FarmRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(fur => new { fur.FarmId, fur.UserId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}