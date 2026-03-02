using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBlackListedTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlacklistedTokens",
                schema: "AgroTempV1",
                table: "BlacklistedTokens");

            migrationBuilder.RenameTable(
                name: "BlacklistedTokens",
                schema: "AgroTempV1",
                newName: "BlacklistedToken",
                newSchema: "AgroTempV1");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AgroTempV1",
                table: "BlacklistedToken",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TokenId",
                schema: "AgroTempV1",
                table: "BlacklistedToken",
                newName: "token_id");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                schema: "AgroTempV1",
                table: "BlacklistedToken",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "BlacklistedAt",
                schema: "AgroTempV1",
                table: "BlacklistedToken",
                newName: "blacklisted_at");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlacklistedToken",
                schema: "AgroTempV1",
                table: "BlacklistedToken",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlacklistedToken",
                schema: "AgroTempV1",
                table: "BlacklistedToken");

            migrationBuilder.RenameTable(
                name: "BlacklistedToken",
                schema: "AgroTempV1",
                newName: "BlacklistedTokens",
                newSchema: "AgroTempV1");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "AgroTempV1",
                table: "BlacklistedTokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "token_id",
                schema: "AgroTempV1",
                table: "BlacklistedTokens",
                newName: "TokenId");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                schema: "AgroTempV1",
                table: "BlacklistedTokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "blacklisted_at",
                schema: "AgroTempV1",
                table: "BlacklistedTokens",
                newName: "BlacklistedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlacklistedTokens",
                schema: "AgroTempV1",
                table: "BlacklistedTokens",
                column: "Id");
        }
    }
}
