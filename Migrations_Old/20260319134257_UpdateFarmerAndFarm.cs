using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFarmerAndFarm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dispute_reports_Farmer_farmer_id",
                schema: "AgroTempV1",
                table: "dispute_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_dispute_reports_Job_Post_job_post_id",
                schema: "AgroTempV1",
                table: "dispute_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_dispute_reports_User_resolved_by_id",
                schema: "AgroTempV1",
                table: "dispute_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_dispute_reports_Worker_worker_id",
                schema: "AgroTempV1",
                table: "dispute_reports");

            migrationBuilder.DropIndex(
                name: "IX_DeviceToken_user_id_expo_push_token",
                schema: "AgroTempV1",
                table: "DeviceToken");

            migrationBuilder.DropColumn(
                name: "cooperative_affiliation",
                schema: "AgroTempV1",
                table: "Farmer");

            migrationBuilder.DropColumn(
                name: "farm_type",
                schema: "AgroTempV1",
                table: "Farmer");

            migrationBuilder.DropColumn(
                name: "organization_name",
                schema: "AgroTempV1",
                table: "Farmer");

            migrationBuilder.AddColumn<string>(
                name: "farm_type",
                schema: "AgroTempV1",
                table: "Farm",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "expo_push_token",
                schema: "AgroTempV1",
                table: "DeviceToken",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceToken_user_id",
                schema: "AgroTempV1",
                table: "DeviceToken",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dispute_reports_Farmer_farmer_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "farmer_id",
                principalSchema: "AgroTempV1",
                principalTable: "Farmer",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_dispute_reports_Job_Post_job_post_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "job_post_id",
                principalSchema: "AgroTempV1",
                principalTable: "Job_Post",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dispute_reports_User_resolved_by_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "resolved_by_id",
                principalSchema: "AgroTempV1",
                principalTable: "User",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_dispute_reports_Worker_worker_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "worker_id",
                principalSchema: "AgroTempV1",
                principalTable: "Worker",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dispute_reports_Farmer_farmer_id",
                schema: "AgroTempV1",
                table: "dispute_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_dispute_reports_Job_Post_job_post_id",
                schema: "AgroTempV1",
                table: "dispute_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_dispute_reports_User_resolved_by_id",
                schema: "AgroTempV1",
                table: "dispute_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_dispute_reports_Worker_worker_id",
                schema: "AgroTempV1",
                table: "dispute_reports");

            migrationBuilder.DropIndex(
                name: "IX_DeviceToken_user_id",
                schema: "AgroTempV1",
                table: "DeviceToken");

            migrationBuilder.DropColumn(
                name: "farm_type",
                schema: "AgroTempV1",
                table: "Farm");

            migrationBuilder.AddColumn<string>(
                name: "cooperative_affiliation",
                schema: "AgroTempV1",
                table: "Farmer",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "farm_type",
                schema: "AgroTempV1",
                table: "Farmer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "organization_name",
                schema: "AgroTempV1",
                table: "Farmer",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "expo_push_token",
                schema: "AgroTempV1",
                table: "DeviceToken",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceToken_user_id_expo_push_token",
                schema: "AgroTempV1",
                table: "DeviceToken",
                columns: new[] { "user_id", "expo_push_token" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_dispute_reports_Farmer_farmer_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "farmer_id",
                principalSchema: "AgroTempV1",
                principalTable: "Farmer",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dispute_reports_Job_Post_job_post_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "job_post_id",
                principalSchema: "AgroTempV1",
                principalTable: "Job_Post",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dispute_reports_User_resolved_by_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "resolved_by_id",
                principalSchema: "AgroTempV1",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dispute_reports_Worker_worker_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "worker_id",
                principalSchema: "AgroTempV1",
                principalTable: "Worker",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
