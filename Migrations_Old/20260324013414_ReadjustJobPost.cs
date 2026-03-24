using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ReadjustJobPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Rename the column first
            migrationBuilder.RenameColumn(
                name: "requirementes",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "requirements");

            // 2. Handle Date Time changes
            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            // 3. FIX: Drop default constraints before altering types
            migrationBuilder.Sql("ALTER TABLE \"AgroTempV1\".\"Job_Post\" ALTER COLUMN \"privileges\" DROP DEFAULT;");
            migrationBuilder.Sql("ALTER TABLE \"AgroTempV1\".\"Job_Post\" ALTER COLUMN \"requirements\" DROP DEFAULT;");

            // 4. Alter columns with USING string_to_array
            migrationBuilder.Sql(
                "ALTER TABLE \"AgroTempV1\".\"Job_Post\" ALTER COLUMN \"privileges\" TYPE text[] USING string_to_array(\"privileges\", ',');"
            );

            migrationBuilder.Sql(
                "ALTER TABLE \"AgroTempV1\".\"Job_Post\" ALTER COLUMN \"requirements\" TYPE text[] USING string_to_array(\"requirements\", ',');"
            );

            // 5. Optional: Set new defaults for the array columns
            migrationBuilder.Sql("ALTER TABLE \"AgroTempV1\".\"Job_Post\" ALTER COLUMN \"privileges\" SET DEFAULT '{}';");
            migrationBuilder.Sql("ALTER TABLE \"AgroTempV1\".\"Job_Post\" ALTER COLUMN \"requirements\" SET DEFAULT '{}';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "requirements",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "requirementes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "privileges",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "requirementes",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]");
        }
    }
}
