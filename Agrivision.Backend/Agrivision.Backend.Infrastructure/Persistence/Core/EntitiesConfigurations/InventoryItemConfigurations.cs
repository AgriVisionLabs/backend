using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class InventoryItemConfigurations : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .ValueGeneratedOnAdd();

        builder.Property(item => item.ItemName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(item => item.Category)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Property(item => item.ThresholdQuantity)
            .IsRequired();

        builder.Property(item => item.UnitCost)
            .IsRequired();

        builder.Property(item => item.MeasurementUnit)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(item => item.ExpirationDate);

        builder.HasOne(item => item.Farm)
            .WithMany()
            .HasForeignKey(item => item.FarmId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Field)
            .WithMany()
            .HasForeignKey(item => item.FieldId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}