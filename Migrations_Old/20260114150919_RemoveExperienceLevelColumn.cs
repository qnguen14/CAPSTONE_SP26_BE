using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExperienceLevelColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceLevel",
                schema: "AgroTempV1",
                table: "Worker_Profile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExperienceLevel",
                schema: "AgroTempV1",
                table: "Worker_Profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
