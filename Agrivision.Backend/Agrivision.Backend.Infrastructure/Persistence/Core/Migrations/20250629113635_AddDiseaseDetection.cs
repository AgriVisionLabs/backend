using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDiseaseDetection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiseaseDetections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfidenceLevel = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PlantedCropId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CropDiseaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_DiseaseDetections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiseaseDetections_CropDiseases_CropDiseaseId",
                        column: x => x.CropDiseaseId,
                        principalTable: "CropDiseases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiseaseDetections_PlantedCrops_PlantedCropId",
                        column: x => x.PlantedCropId,
                        principalTable: "PlantedCrops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseDetections_CropDiseaseId",
                table: "DiseaseDetections",
                column: "CropDiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseDetections_Id",
                table: "DiseaseDetections",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseDetections_PlantedCropId",
                table: "DiseaseDetections",
                column: "PlantedCropId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiseaseDetections");
        }
    }
}
