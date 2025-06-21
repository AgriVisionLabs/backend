using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class IrrigationEventConfigurations : IEntityTypeConfiguration<IrrigationEvent>
{
    public void Configure(EntityTypeBuilder<IrrigationEvent> builder)
    {
        builder.ToTable("IrrigationEvents");

        builder.HasKey(ie => ie.Id);

        builder.Property(ie => ie.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(ie => ie.StartTime)
            .IsRequired();

        builder.Property(ie => ie.TriggerMethod)
            .HasConversion<int>()
            .IsRequired();
        
        builder.HasOne(ie => ie.IrrigationUnit)
            .WithMany(iu => iu.IrrigationEvents)
            .HasForeignKey(ie => ie.IrrigationUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}