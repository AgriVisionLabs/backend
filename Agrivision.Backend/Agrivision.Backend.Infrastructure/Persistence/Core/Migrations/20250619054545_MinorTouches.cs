using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class MinorTouches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TaskItem_FieldId_Title_Unique_Incomplete_NotDeleted",
                table: "TaskItems",
                columns: new[] { "FieldId", "Title" },
                unique: true,
                filter: "[CompletedAt] IS NULL AND [IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskItem_FieldId_Title_Unique_Incomplete_NotDeleted",
                table: "TaskItems");
        }
    }
}
