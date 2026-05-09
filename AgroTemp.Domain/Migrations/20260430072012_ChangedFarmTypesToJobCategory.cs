using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFarmTypesToJobCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "farm_type",
                schema: "AgroTempV3",
                table: "Farm");

            migrationBuilder.AddColumn<Guid?>(
                name: "farm_type_id",
                schema: "AgroTempV3",
                table: "Farm",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Farm_farm_type_id",
                schema: "AgroTempV3",
                table: "Farm",
                column: "farm_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_Job_Category_farm_type_id",
                schema: "AgroTempV3",
                table: "Farm",
                column: "farm_type_id",
                principalSchema: "AgroTempV3",
                principalTable: "Job_Category",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farm_Job_Category_farm_type_id",
                schema: "AgroTempV3",
                table: "Farm");

            migrationBuilder.DropIndex(
                name: "IX_Farm_farm_type_id",
                schema: "AgroTempV3",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "farm_type_id",
                schema: "AgroTempV3",
                table: "Farm");

            migrationBuilder.AddColumn<int>(
                name: "farm_type",
                schema: "AgroTempV3",
                table: "Farm",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
