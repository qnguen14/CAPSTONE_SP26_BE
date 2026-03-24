using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSkillCategoryToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add a temporary column to hold the new integer values
            migrationBuilder.AddColumn<int>(
                name: "category_temp",
                schema: "AgroTempV1",
                table: "Skill",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            // Map existing string values to enum integers
            migrationBuilder.Sql(@"
                UPDATE ""AgroTempV1"".""Skill""
                SET category_temp = CASE 
                    WHEN category = 'Agronomy' THEN 1
                    WHEN category = 'Animal Husbandry' THEN 2
                    WHEN category = 'Aquiculture' THEN 3
                    ELSE 1
                END
                WHERE category IS NOT NULL;
            ");

            // Drop the old string column
            migrationBuilder.DropColumn(
                name: "category",
                schema: "AgroTempV1",
                table: "Skill");

            // Rename the temporary column to the original name
            migrationBuilder.RenameColumn(
                name: "category_temp",
                schema: "AgroTempV1",
                table: "Skill",
                newName: "category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add a temporary column to hold the old string values
            migrationBuilder.AddColumn<string>(
                name: "category_temp",
                schema: "AgroTempV1",
                table: "Skill",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "Agronomy");

            // Map integer values back to strings
            migrationBuilder.Sql(@"
                UPDATE ""AgroTempV1"".""Skill""
                SET category_temp = CASE 
                    WHEN category = 1 THEN 'Agronomy'
                    WHEN category = 2 THEN 'Animal Husbandry'
                    WHEN category = 3 THEN 'Aquiculture'
                    ELSE 'Agronomy'
                END;
            ");

            // Drop the integer column
            migrationBuilder.DropColumn(
                name: "category",
                schema: "AgroTempV1",
                table: "Skill");

            // Rename the temporary column back to the original name
            migrationBuilder.RenameColumn(
                name: "category_temp",
                schema: "AgroTempV1",
                table: "Skill",
                newName: "category");
        }
    }
}
