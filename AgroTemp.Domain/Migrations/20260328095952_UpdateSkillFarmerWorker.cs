using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSkillFarmerWorker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Truncate existing Skills and their dependencies to avoid constraint violations
            // since we are replacing the category ID mechanism.
            migrationBuilder.Sql("TRUNCATE TABLE \"AgroTempV2\".\"Skill\" CASCADE;");

            migrationBuilder.DropColumn(
                name: "age_range",
                schema: "AgroTempV2",
                table: "Worker");

            migrationBuilder.DropColumn(
                name: "address",
                schema: "AgroTempV2",
                table: "User");

            migrationBuilder.DropColumn(
                name: "category",
                schema: "AgroTempV2",
                table: "Skill");

            migrationBuilder.AddColumn<string>(
                name: "address",
                schema: "AgroTempV2",
                table: "Worker",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "date_of_birth",
                schema: "AgroTempV2",
                table: "Worker",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<Guid>(
                name: "job_category_id",
                schema: "AgroTempV2",
                table: "Skill",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "address",
                schema: "AgroTempV2",
                table: "Farmer",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "date_of_birth",
                schema: "AgroTempV2",
                table: "Farmer",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_Skill_job_category_id",
                schema: "AgroTempV2",
                table: "Skill",
                column: "job_category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Job_Category_job_category_id",
                schema: "AgroTempV2",
                table: "Skill",
                column: "job_category_id",
                principalSchema: "AgroTempV2",
                principalTable: "Job_Category",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skill_Job_Category_job_category_id",
                schema: "AgroTempV2",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Skill_job_category_id",
                schema: "AgroTempV2",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "address",
                schema: "AgroTempV2",
                table: "Worker");

            migrationBuilder.DropColumn(
                name: "date_of_birth",
                schema: "AgroTempV2",
                table: "Worker");

            migrationBuilder.DropColumn(
                name: "job_category_id",
                schema: "AgroTempV2",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "address",
                schema: "AgroTempV2",
                table: "Farmer");

            migrationBuilder.DropColumn(
                name: "date_of_birth",
                schema: "AgroTempV2",
                table: "Farmer");

            migrationBuilder.AddColumn<string>(
                name: "age_range",
                schema: "AgroTempV2",
                table: "Worker",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "address",
                schema: "AgroTempV2",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "category",
                schema: "AgroTempV2",
                table: "Skill",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
