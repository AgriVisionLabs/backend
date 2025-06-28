using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class minor7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Yield",
                table: "PlantedCrops",
                type: "float(18)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Yield",
                table: "PlantedCrops");
        }
    }
}
