using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class SafeFarmDeletionWithEscrowRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job_Post_Farm_farm_id",
                schema: "AgroTempV3",
                table: "Job_Post");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Post_Farm_farm_id",
                schema: "AgroTempV3",
                table: "Job_Post",
                column: "farm_id",
                principalSchema: "AgroTempV3",
                principalTable: "Farm",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job_Post_Farm_farm_id",
                schema: "AgroTempV3",
                table: "Job_Post");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Post_Farm_farm_id",
                schema: "AgroTempV3",
                table: "Job_Post",
                column: "farm_id",
                principalSchema: "AgroTempV3",
                principalTable: "Farm",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
