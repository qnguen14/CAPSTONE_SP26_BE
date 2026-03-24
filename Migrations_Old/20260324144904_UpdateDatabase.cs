using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "AgroTempV1",
                table: "WalletTransactions",
                newName: "type");

            migrationBuilder.AlterColumn<int>(
                name: "type",
                schema: "AgroTempV1",
                table: "WalletTransactions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "type",
                schema: "AgroTempV1",
                table: "WalletTransactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.RenameColumn(
                name: "type",
                schema: "AgroTempV1",
                table: "WalletTransactions",
                newName: "Type");
        }
    }
}
