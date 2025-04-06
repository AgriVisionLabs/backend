using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldsIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Fields_Name_CreatedById",
                table: "Fields");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_Name_FarmId",
                table: "Fields",
                columns: new[] { "Name", "FarmId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Fields_Name_FarmId",
                table: "Fields");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Fields",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_Name_CreatedById",
                table: "Fields",
                columns: new[] { "Name", "CreatedById" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
