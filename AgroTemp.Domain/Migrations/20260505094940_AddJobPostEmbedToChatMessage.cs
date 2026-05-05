using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddJobPostEmbedToChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Report_Comments_Job_Post_job_post_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "job_post_id",
                schema: "AgroTempV3",
                table: "Chat_Message",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Message_job_post_id",
                schema: "AgroTempV3",
                table: "Chat_Message",
                column: "job_post_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Message_Job_Post_job_post_id",
                schema: "AgroTempV3",
                table: "Chat_Message",
                column: "job_post_id",
                principalSchema: "AgroTempV3",
                principalTable: "Job_Post",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_Report_Comments_Job_Post_job_post_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments",
                column: "job_post_id",
                principalSchema: "AgroTempV3",
                principalTable: "Job_Post",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Message_Job_Post_job_post_id",
                schema: "AgroTempV3",
                table: "Chat_Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Report_Comments_Job_Post_job_post_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments");

            migrationBuilder.DropIndex(
                name: "IX_Chat_Message_job_post_id",
                schema: "AgroTempV3",
                table: "Chat_Message");

            migrationBuilder.DropColumn(
                name: "job_post_id",
                schema: "AgroTempV3",
                table: "Chat_Message");

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
    }
}
