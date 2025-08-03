using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddPfp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PfpImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PfpImageUrl",
                table: "AspNetUsers");
        }
    }
}
