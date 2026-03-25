using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddImageListToFarm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""AgroTempV2"".""Farm"" ALTER COLUMN image_url TYPE text[] USING string_to_array(image_url, ',')::text[];");

            migrationBuilder.AlterColumn<List<string>>(
                name: "image_url",
                schema: "AgroTempV2",
                table: "Farm",
                type: "text[]",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""AgroTempV2"".""Farm"" ALTER COLUMN image_url TYPE character varying(1024) USING array_to_string(image_url, ',');");

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                schema: "AgroTempV2",
                table: "Farm",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldMaxLength: 1024,
                oldNullable: true);
        }
    }
}
