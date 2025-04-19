using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Core.Migrations
{
    /// <inheritdoc />
    public partial class MinorChangesToInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FarmInvitations_FarmId_InvitedEmail",
                table: "FarmInvitations");

            migrationBuilder.CreateIndex(
                name: "IX_FarmInvitations_FarmId_InvitedEmail",
                table: "FarmInvitations",
                columns: new[] { "FarmId", "InvitedEmail" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [IsAccepted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FarmInvitations_FarmId_InvitedEmail",
                table: "FarmInvitations");

            migrationBuilder.CreateIndex(
                name: "IX_FarmInvitations_FarmId_InvitedEmail",
                table: "FarmInvitations",
                columns: new[] { "FarmId", "InvitedEmail" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
