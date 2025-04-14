using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class FarmInvitationConfigurations : IEntityTypeConfiguration<FarmInvitation>
{
    public void Configure(EntityTypeBuilder<FarmInvitation> builder)
    {
        builder.ToTable("FarmInvitations");

        builder.HasKey(inv => inv.Id);

        builder.Property(inv => inv.Id)
            .ValueGeneratedOnAdd();

        builder.Property(inv => inv.FarmId)
            .IsRequired();

        builder.Property(inv => inv.FarmRoleId)
            .IsRequired();

        builder.Property(inv => inv.InvitedEmail)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(inv => inv.Token)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(inv => inv.IsAccepted)
            .IsRequired();

        builder.Property(inv => inv.ExpiresAt)
            .IsRequired();

        builder.Property(inv => inv.AcceptedAt)
            .IsRequired(false);

        builder.HasIndex(inv => inv.Token)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(inv => new { inv.FarmId, inv.InvitedEmail })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne(inv => inv.Farm)
            .WithMany(f => f.FarmInvitations)
            .HasForeignKey(inv => inv.FarmId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(inv => inv.FarmRole)
            .WithMany()
            .HasForeignKey(inv => inv.FarmRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}