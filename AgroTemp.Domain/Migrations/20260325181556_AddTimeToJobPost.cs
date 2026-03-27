using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeToJobPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "start_date",
                schema: "AgroTempV2",
                table: "Job_Post",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<DateOnly>>(
                name: "selected_days",
                schema: "AgroTempV2",
                table: "Job_Post",
                type: "date[]",
                nullable: false,
                oldClrType: typeof(List<DateTime>),
                oldType: "timestamp with time zone[]");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "end_date",
                schema: "AgroTempV2",
                table: "Job_Post",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "end_time",
                schema: "AgroTempV2",
                table: "Job_Post",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "start_time",
                schema: "AgroTempV2",
                table: "Job_Post",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_time",
                schema: "AgroTempV2",
                table: "Job_Post");

            migrationBuilder.DropColumn(
                name: "start_time",
                schema: "AgroTempV2",
                table: "Job_Post");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                schema: "AgroTempV2",
                table: "Job_Post",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<DateTime>>(
                name: "selected_days",
                schema: "AgroTempV2",
                table: "Job_Post",
                type: "timestamp with time zone[]",
                nullable: false,
                oldClrType: typeof(List<DateOnly>),
                oldType: "date[]");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                schema: "AgroTempV2",
                table: "Job_Post",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
