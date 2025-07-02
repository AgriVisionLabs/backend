

using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;
public class UserSubscriptionConfigurations : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions");

        builder.HasKey(us => us.Id);

        builder.Property(us => us.Id)
            .ValueGeneratedOnAdd();

        builder.Property(us => us.SubscriptionPlanId)
            .IsRequired();

        builder.Property(us => us.Status)
            .IsRequired();

        builder.Property(us => us.StartDate)
            .IsRequired();

        builder.Property(us => us.EndDate)
            .IsRequired();

        builder.HasOne(us => us.SubscriptionPlan)
            .WithMany()
            .HasForeignKey(us => us.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
