using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class Minor2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorUnits_DeviceId",
                table: "SensorUnits");

            migrationBuilder.DropIndex(
                name: "IX_SensorUnits_Name",
                table: "SensorUnits");

            migrationBuilder.CreateIndex(
                name: "IX_SensorUnits_DeviceId",
                table: "SensorUnits",
                column: "DeviceId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SensorUnits_FarmId_Name",
                table: "SensorUnits",
                columns: new[] { "FarmId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorUnits_DeviceId",
                table: "SensorUnits");

            migrationBuilder.DropIndex(
                name: "IX_SensorUnits_FarmId_Name",
                table: "SensorUnits");

            migrationBuilder.CreateIndex(
                name: "IX_SensorUnits_DeviceId",
                table: "SensorUnits",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SensorUnits_Name",
                table: "SensorUnits",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
