using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;
public class SubscriptionPlanConfigurations : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id)
            .ValueGeneratedOnAdd();

        builder.Property(sp => sp.Name)
            .IsRequired();

        builder.Property(sp => sp.Price)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(sp => sp.Currency)
            .IsRequired();
        
        builder.Property(sp => sp.MaxFarms)
            .IsRequired();
        
        builder.Property(sp => sp.MaxFields)
            .IsRequired();

        builder.Property(sp => sp.UnlimitedAiFeatureUsage)
           .IsRequired();

        builder.Property(sp => sp.IsActive)
           .IsRequired();
    }
}
