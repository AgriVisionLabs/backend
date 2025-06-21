using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddIrrigationEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorConfigurations_IrrigationUnits_IrrigationUnitId",
                table: "SensorConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_SensorConfigurations_IrrigationUnitId",
                table: "SensorConfigurations");

            migrationBuilder.DropColumn(
                name: "IrrigationUnitId",
                table: "SensorConfigurations");

            migrationBuilder.CreateTable(
                name: "IrrigationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IrrigationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TriggerMethod = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrrigationEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrrigationEvents_IrrigationUnits_IrrigationUnitId",
                        column: x => x.IrrigationUnitId,
                        principalTable: "IrrigationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationEvents_IrrigationUnitId",
                table: "IrrigationEvents",
                column: "IrrigationUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IrrigationEvents");

            migrationBuilder.AddColumn<Guid>(
                name: "IrrigationUnitId",
                table: "SensorConfigurations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SensorConfigurations_IrrigationUnitId",
                table: "SensorConfigurations",
                column: "IrrigationUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorConfigurations_IrrigationUnits_IrrigationUnitId",
                table: "SensorConfigurations",
                column: "IrrigationUnitId",
                principalTable: "IrrigationUnits",
                principalColumn: "Id");
        }
    }
}
