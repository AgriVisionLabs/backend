using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_farmMembers_Farms_FarmId",
                table: "farmMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_farmMembers",
                table: "farmMembers");

            migrationBuilder.RenameTable(
                name: "farmMembers",
                newName: "FarmMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FarmMembers",
                table: "FarmMembers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FarmMembers_FarmId",
                table: "FarmMembers",
                column: "FarmId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmMembers_Farms_FarmId",
                table: "FarmMembers",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmMembers_Farms_FarmId",
                table: "FarmMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FarmMembers",
                table: "FarmMembers");

            migrationBuilder.DropIndex(
                name: "IX_FarmMembers_FarmId",
                table: "FarmMembers");

            migrationBuilder.RenameTable(
                name: "FarmMembers",
                newName: "farmMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_farmMembers",
                table: "farmMembers",
                columns: new[] { "FarmId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_farmMembers_Farms_FarmId",
                table: "farmMembers",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
