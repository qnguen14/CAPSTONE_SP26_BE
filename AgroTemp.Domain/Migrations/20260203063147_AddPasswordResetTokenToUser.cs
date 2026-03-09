using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokenToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password_reset_token",
                schema: "AgroTempV1",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "password_reset_token_expires_at",
                schema: "AgroTempV1",
                table: "User",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_reset_token",
                schema: "AgroTempV1",
                table: "User");

            migrationBuilder.DropColumn(
                name: "password_reset_token_expires_at",
                schema: "AgroTempV1",
                table: "User");
        }
    }
}
