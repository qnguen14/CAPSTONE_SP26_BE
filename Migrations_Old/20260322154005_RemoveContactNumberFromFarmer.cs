using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveContactNumberFromFarmer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contact_number",
                schema: "AgroTempV1",
                table: "Farmer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "contact_number",
                schema: "AgroTempV1",
                table: "Farmer",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }
    }
}
