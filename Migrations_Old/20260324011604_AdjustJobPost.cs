using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AdjustJobPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estimated_hours",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.RenameColumn(
                name: "preferences",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "requirementes");

            migrationBuilder.AddColumn<string>(
                name: "privileges",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<DateTime>>(
                name: "selected_days",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "timestamp with time zone[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "privileges",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.DropColumn(
                name: "selected_days",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.RenameColumn(
                name: "requirementes",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "preferences");

            migrationBuilder.AddColumn<decimal>(
                name: "estimated_hours",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
