using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Identity.Migrations
{
    /// <inheritdoc />
    public partial class OtpVerficationTableAddingIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_Email_CreatedOn",
                table: "OtpVerifications",
                columns: new[] { "Email", "CreatedOn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_Email_CreatedOn",
                table: "OtpVerifications");
        }
    }
}
