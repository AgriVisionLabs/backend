using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class MinorChanges4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_FarmId",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "ItemName",
                table: "InventoryItems",
                newName: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_FarmId_Name",
                table: "InventoryItems",
                columns: new[] { "FarmId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_FarmId_Name",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "InventoryItems",
                newName: "ItemName");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_FarmId",
                table: "InventoryItems",
                column: "FarmId");
        }
    }
}
