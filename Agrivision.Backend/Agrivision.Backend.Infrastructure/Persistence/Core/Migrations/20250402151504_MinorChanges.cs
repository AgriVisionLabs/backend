using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class MinorChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Farms_Name_CreatedById",
                table: "Farms");

            migrationBuilder.CreateIndex(
                name: "IX_Farms_Name_CreatedById",
                table: "Farms",
                columns: new[] { "Name", "CreatedById" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Farms_Name_CreatedById",
                table: "Farms");

            migrationBuilder.CreateIndex(
                name: "IX_Farms_Name_CreatedById",
                table: "Farms",
                columns: new[] { "Name", "CreatedById" },
                unique: true);
        }
    }
}
