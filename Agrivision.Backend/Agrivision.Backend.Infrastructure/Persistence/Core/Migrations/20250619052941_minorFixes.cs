using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class minorFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_TaskItem_OnlyOneAssignedOrClaimed",
                table: "TaskItems");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TaskItem_OnlyOneAssignedOrClaimed",
                table: "TaskItems",
                sql: "(([AssignedToId] IS NULL AND [ClaimedById] IS NOT NULL) OR ([AssignedToId] IS NOT NULL AND [ClaimedById] IS NULL) OR ([AssignedToId] IS NULL AND [ClaimedById] IS NULL))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_TaskItem_OnlyOneAssignedOrClaimed",
                table: "TaskItems");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TaskItem_OnlyOneAssignedOrClaimed",
                table: "TaskItems",
                sql: "(([AssignedToId] IS NULL AND [ClaimedById] IS NOT NULL) OR ([AssignedToId] IS NOT NULL AND [ClaimedById] IS NULL))");
        }
    }
}
