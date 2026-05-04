using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddJobPostEmbedToDisputeComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "job_post_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_Report_Comments_job_post_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments",
                column: "job_post_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_Report_Comments_Job_Post_job_post_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments",
                column: "job_post_id",
                principalSchema: "AgroTempV3",
                principalTable: "Job_Post",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Report_Comments_Job_Post_job_post_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments");

            migrationBuilder.DropIndex(
                name: "IX_Dispute_Report_Comments_job_post_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments");

            migrationBuilder.DropColumn(
                name: "job_post_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments");
        }
    }
}
