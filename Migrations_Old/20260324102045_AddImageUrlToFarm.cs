using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToFarm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<DateTime>>(
                name: "work_dates",
                schema: "AgroTempV1",
                table: "Job_Application",
                type: "timestamp with time zone[]",
                nullable: true,
                oldClrType: typeof(List<DateTime>),
                oldType: "timestamp with time zone[]");

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                schema: "AgroTempV1",
                table: "Farm",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                schema: "AgroTempV1",
                table: "Farm");

            migrationBuilder.AlterColumn<List<DateTime>>(
                name: "work_dates",
                schema: "AgroTempV1",
                table: "Job_Application",
                type: "timestamp with time zone[]",
                nullable: false,
                oldClrType: typeof(List<DateTime>),
                oldType: "timestamp with time zone[]",
                oldNullable: true);
        }
    }
}
