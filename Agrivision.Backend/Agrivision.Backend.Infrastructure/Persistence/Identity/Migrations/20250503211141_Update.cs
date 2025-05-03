using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agrivision.Backend.Infrastructure.Persistence.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_Email_OtpCode",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "OtpCode",
                table: "OtpVerifications");

            migrationBuilder.RenameColumn(
                name: "ExpiresOn",
                table: "OtpVerifications",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "OtpVerifications",
                newName: "UserId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                table: "OtpVerifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OtpVerifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "OtpVerifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "OtpVerifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashedOtpCode",
                table: "OtpVerifications",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "OtpVerifications",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "OtpVerifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "OtpVerifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OtpVerifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_DeletedById",
                table: "OtpVerifications",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_ExpiresAt",
                table: "OtpVerifications",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_UserId_Purpose",
                table: "OtpVerifications",
                columns: new[] { "UserId", "Purpose" },
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Otp_DeletedBy",
                table: "OtpVerifications",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Otp_User",
                table: "OtpVerifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Otp_DeletedBy",
                table: "OtpVerifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Otp_User",
                table: "OtpVerifications");

            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_DeletedById",
                table: "OtpVerifications");

            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_ExpiresAt",
                table: "OtpVerifications");

            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_UserId_Purpose",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "HashedOtpCode",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OtpVerifications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OtpVerifications",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "OtpVerifications",
                newName: "ExpiresOn");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                table: "OtpVerifications",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "OtpVerifications",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                table: "OtpVerifications",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_Email_OtpCode",
                table: "OtpVerifications",
                columns: new[] { "Email", "OtpCode" });
        }
    }
}
