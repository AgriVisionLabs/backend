using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class InventoryItemTransactionConfigurations : IEntityTypeConfiguration<InventoryItemTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryItemTransaction> builder)
    {
        builder.ToTable("InventoryItemTransactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.QuantityChanged)
            .IsRequired();

        builder.Property(t => t.Reason)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(t => t.InventoryItem)
            .WithMany(item => item.Transactions)
            .HasForeignKey(t => t.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}