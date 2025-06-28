using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class Minor6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualYield",
                table: "PlantedCrops");

            migrationBuilder.DropColumn(
                name: "EstimatedYield",
                table: "PlantedCrops");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ActualYield",
                table: "PlantedCrops",
                type: "float(18)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedYield",
                table: "PlantedCrops",
                type: "float(18)",
                precision: 18,
                scale: 2,
                nullable: true);
        }
    }
}
