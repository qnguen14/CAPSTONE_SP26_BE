using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJobPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "age_requirement",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.DropColumn(
                name: "latitude",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.DropColumn(
                name: "longitude",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.AddColumn<Guid>(
                name: "farm_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Job_Post_farm_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                column: "farm_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Post_Farm_farm_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                column: "farm_id",
                principalSchema: "AgroTempV1",
                principalTable: "Farm",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job_Post_Farm_farm_id",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.DropIndex(
                name: "IX_Job_Post_farm_id",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.DropColumn(
                name: "farm_id",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.AddColumn<decimal>(
                name: "age_requirement",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "latitude",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "longitude",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
