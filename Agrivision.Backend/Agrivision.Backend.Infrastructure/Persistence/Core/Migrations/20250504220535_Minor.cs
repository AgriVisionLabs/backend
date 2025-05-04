using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class Minor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComparisonOperator",
                table: "AutomationRules");

            migrationBuilder.RenameColumn(
                name: "ThresholdValue",
                table: "AutomationRules",
                newName: "MinimumThresholdValue");

            migrationBuilder.AddColumn<float>(
                name: "MaximumThresholdValue",
                table: "AutomationRules",
                type: "real",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SensorUnits_Name",
                table: "SensorUnits",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AutomationRules_Name",
                table: "AutomationRules",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorUnits_Name",
                table: "SensorUnits");

            migrationBuilder.DropIndex(
                name: "IX_AutomationRules_Name",
                table: "AutomationRules");

            migrationBuilder.DropColumn(
                name: "MaximumThresholdValue",
                table: "AutomationRules");

            migrationBuilder.RenameColumn(
                name: "MinimumThresholdValue",
                table: "AutomationRules",
                newName: "ThresholdValue");

            migrationBuilder.AddColumn<string>(
                name: "ComparisonOperator",
                table: "AutomationRules",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
