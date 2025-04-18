using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddIrrigationUnitAndIrrigationUnitDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IrrigationUnitDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MacAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirmwareVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ManufacturedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAssigned = table.Column<bool>(type: "bit", nullable: false),
                    IsAssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProvisioningKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Used to verify secure ownership during manual provisioning."),
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
                    table.PrimaryKey("PK_IrrigationUnitDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IrrigationUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InstallationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastActivation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDeactivation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastMaintenance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastMaintenanceCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMaintenance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_IrrigationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrrigationUnits_Farms_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IrrigationUnits_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IrrigationUnits_IrrigationUnitDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "IrrigationUnitDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationUnitDevices_MacAddress",
                table: "IrrigationUnitDevices",
                column: "MacAddress",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationUnitDevices_ProvisioningKey",
                table: "IrrigationUnitDevices",
                column: "ProvisioningKey",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationUnitDevices_SerialNumber",
                table: "IrrigationUnitDevices",
                column: "SerialNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationUnits_DeviceId",
                table: "IrrigationUnits",
                column: "DeviceId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationUnits_FarmId_Name",
                table: "IrrigationUnits",
                columns: new[] { "FarmId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IrrigationUnits_FieldId",
                table: "IrrigationUnits",
                column: "FieldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IrrigationUnits");

            migrationBuilder.DropTable(
                name: "IrrigationUnitDevices");
        }
    }
}
