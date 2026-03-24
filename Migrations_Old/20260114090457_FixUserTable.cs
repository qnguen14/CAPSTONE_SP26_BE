using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FixUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farmer_Profile_Users_user_id",
                schema: "AgroTempV1",
                table: "Farmer_Profile");

            migrationBuilder.DropForeignKey(
                name: "FK_Worker_Profile_Users_user_id",
                schema: "AgroTempV1",
                table: "Worker_Profile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                schema: "AgroTempV1",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "AgroTempV1",
                newName: "User",
                newSchema: "AgroTempV1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                schema: "AgroTempV1",
                table: "User",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Farmer_Profile_User_user_id",
                schema: "AgroTempV1",
                table: "Farmer_Profile",
                column: "user_id",
                principalSchema: "AgroTempV1",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Worker_Profile_User_user_id",
                schema: "AgroTempV1",
                table: "Worker_Profile",
                column: "user_id",
                principalSchema: "AgroTempV1",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farmer_Profile_User_user_id",
                schema: "AgroTempV1",
                table: "Farmer_Profile");

            migrationBuilder.DropForeignKey(
                name: "FK_Worker_Profile_User_user_id",
                schema: "AgroTempV1",
                table: "Worker_Profile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                schema: "AgroTempV1",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "AgroTempV1",
                newName: "Users",
                newSchema: "AgroTempV1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                schema: "AgroTempV1",
                table: "Users",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Farmer_Profile_Users_user_id",
                schema: "AgroTempV1",
                table: "Farmer_Profile",
                column: "user_id",
                principalSchema: "AgroTempV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Worker_Profile_Users_user_id",
                schema: "AgroTempV1",
                table: "Worker_Profile",
                column: "user_id",
                principalSchema: "AgroTempV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
