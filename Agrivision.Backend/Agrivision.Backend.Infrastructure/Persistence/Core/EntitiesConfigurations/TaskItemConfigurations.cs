using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Core.EntitiesConfigurations;

public class TaskItemConfigurations : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_TaskItem_OnlyOneAssignedOrClaimed",
                "(([AssignedToId] IS NULL AND [ClaimedById] IS NOT NULL) OR " +
                "([AssignedToId] IS NOT NULL AND [ClaimedById] IS NULL) OR " +
                "([AssignedToId] IS NULL AND [ClaimedById] IS NULL))"
            );
        });

        builder.HasKey(task => task.Id);

        builder.Property(task => task.Id)
            .ValueGeneratedOnAdd();

        builder.Property(task => task.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(task => task.Description)
            .HasMaxLength(1000); // be generous but reasonable

        builder.Property(task => task.AssignedToId)
            .HasMaxLength(100); // consistent with identity keys

        builder.Property(task => task.ClaimedById)
            .HasMaxLength(100);

        builder.Property(task => task.DueDate)
            .IsRequired(false);

        builder.Property(task => task.CompletedAt)
            .IsRequired(false);

        builder.Property(task => task.ItemPriority)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(task => task.Field)
            .WithMany(field => field.TaskItems)
            .HasForeignKey(task => task.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(task => task.FieldId);
        builder.HasIndex(task => task.AssignedToId);
        builder.HasIndex(task => task.DueDate);
        
        // unique title per field
        builder.HasIndex(task => new { task.FieldId, task.Title })
            .IsUnique()
            .HasFilter("[CompletedAt] IS NULL AND [IsDeleted] = 0")
            .HasDatabaseName("IX_TaskItem_FieldId_Title_Unique_Incomplete_NotDeleted");
    }
}