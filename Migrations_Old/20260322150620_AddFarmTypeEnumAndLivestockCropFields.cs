using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFarmTypeEnumAndLivestockCropFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "category",
                schema: "AgroTempV1",
                table: "Skill",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            // Set empty/invalid rows to '1' (Livestock) before casting
            migrationBuilder.Sql(
                @"UPDATE ""AgroTempV1"".""Farm"" SET farm_type = '1' WHERE farm_type IS NULL OR farm_type = '' OR farm_type !~ '^\d+$';");
            migrationBuilder.Sql(
                @"ALTER TABLE ""AgroTempV1"".""Farm"" ALTER COLUMN farm_type DROP DEFAULT;");
            migrationBuilder.Sql(
                @"ALTER TABLE ""AgroTempV1"".""Farm"" ALTER COLUMN farm_type TYPE integer USING farm_type::integer;");

            migrationBuilder.AddColumn<decimal>(
                name: "area_size",
                schema: "AgroTempV1",
                table: "Farm",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "livestock_count",
                schema: "AgroTempV1",
                table: "Farm",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "area_size",
                schema: "AgroTempV1",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "livestock_count",
                schema: "AgroTempV1",
                table: "Farm");

            migrationBuilder.AlterColumn<string>(
                name: "category",
                schema: "AgroTempV1",
                table: "Skill",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "farm_type",
                schema: "AgroTempV1",
                table: "Farm",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
