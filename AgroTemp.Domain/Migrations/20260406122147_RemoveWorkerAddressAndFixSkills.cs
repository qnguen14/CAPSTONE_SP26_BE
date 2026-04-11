using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWorkerAddressAndFixSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                schema: "AgroTempV2",
                table: "Worker");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                schema: "AgroTempV2",
                table: "Worker",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
