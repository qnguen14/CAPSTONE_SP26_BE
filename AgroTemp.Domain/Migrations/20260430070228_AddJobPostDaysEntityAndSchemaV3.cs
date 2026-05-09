using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddJobPostDaysEntityAndSchemaV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selected_days",
                schema: "AgroTempV2",
                table: "Job_Post");

            migrationBuilder.EnsureSchema(
                name: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Worker_Skill",
                schema: "AgroTempV2",
                newName: "Worker_Skill",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Worker_Session",
                schema: "AgroTempV2",
                newName: "Worker_Session",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Worker",
                schema: "AgroTempV2",
                newName: "Worker",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Withdrawal_Requests",
                schema: "AgroTempV2",
                newName: "Withdrawal_Requests",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Wallet_Transactions",
                schema: "AgroTempV2",
                newName: "Wallet_Transactions",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Wallet",
                schema: "AgroTempV2",
                newName: "Wallet",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "AgroTempV2",
                newName: "User",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Skill",
                schema: "AgroTempV2",
                newName: "Skill",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Saved_Job_Post",
                schema: "AgroTempV2",
                newName: "Saved_Job_Post",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Rating",
                schema: "AgroTempV2",
                newName: "Rating",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "PayOS_Webhook_Log",
                schema: "AgroTempV2",
                newName: "PayOS_Webhook_Log",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "PayOS_Transaction",
                schema: "AgroTempV2",
                newName: "PayOS_Transaction",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "PayOS_Order_Item",
                schema: "AgroTempV2",
                newName: "PayOS_Order_Item",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "PayOS_Order",
                schema: "AgroTempV2",
                newName: "PayOS_Order",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "PayOS_Invoice",
                schema: "AgroTempV2",
                newName: "PayOS_Invoice",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Payment",
                schema: "AgroTempV2",
                newName: "Payment",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Notification",
                schema: "AgroTempV2",
                newName: "Notification",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Job_Skill_Requirement",
                schema: "AgroTempV2",
                newName: "Job_Skill_Requirement",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Job_Post",
                schema: "AgroTempV2",
                newName: "Job_Post",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Job_Detail",
                schema: "AgroTempV2",
                newName: "Job_Detail",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Job_Category",
                schema: "AgroTempV2",
                newName: "Job_Category",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "job_attachment",
                schema: "AgroTempV2",
                newName: "job_attachment",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Job_Application",
                schema: "AgroTempV2",
                newName: "Job_Application",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Farmer",
                schema: "AgroTempV2",
                newName: "Farmer",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Farm",
                schema: "AgroTempV2",
                newName: "Farm",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Dispute_Reports",
                schema: "AgroTempV2",
                newName: "Dispute_Reports",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Dispute_Report_Comments",
                schema: "AgroTempV2",
                newName: "Dispute_Report_Comments",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Device_Token",
                schema: "AgroTempV2",
                newName: "Device_Token",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Chat_Message",
                schema: "AgroTempV2",
                newName: "Chat_Message",
                newSchema: "AgroTempV3");

            migrationBuilder.RenameTable(
                name: "Blacklisted_Tokens",
                schema: "AgroTempV2",
                newName: "Blacklisted_Tokens",
                newSchema: "AgroTempV3");

            migrationBuilder.AddColumn<Guid>(
                name: "target_user_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Job_Post_Day",
                schema: "AgroTempV3",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_date = table.Column<DateOnly>(type: "date", nullable: false),
                    workers_needed = table.Column<int>(type: "integer", nullable: false),
                    workers_accepted = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Post_Day", x => x.id);
                    table.ForeignKey(
                        name: "FK_Job_Post_Day_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV3",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Job_Post_Day_job_post_id_work_date",
                schema: "AgroTempV3",
                table: "Job_Post_Day",
                columns: new[] { "job_post_id", "work_date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Job_Post_Day",
                schema: "AgroTempV3");

            migrationBuilder.DropColumn(
                name: "target_user_id",
                schema: "AgroTempV3",
                table: "Dispute_Report_Comments");

            migrationBuilder.EnsureSchema(
                name: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Worker_Skill",
                schema: "AgroTempV3",
                newName: "Worker_Skill",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Worker_Session",
                schema: "AgroTempV3",
                newName: "Worker_Session",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Worker",
                schema: "AgroTempV3",
                newName: "Worker",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Withdrawal_Requests",
                schema: "AgroTempV3",
                newName: "Withdrawal_Requests",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Wallet_Transactions",
                schema: "AgroTempV3",
                newName: "Wallet_Transactions",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Wallet",
                schema: "AgroTempV3",
                newName: "Wallet",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "AgroTempV3",
                newName: "User",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Skill",
                schema: "AgroTempV3",
                newName: "Skill",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Saved_Job_Post",
                schema: "AgroTempV3",
                newName: "Saved_Job_Post",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Rating",
                schema: "AgroTempV3",
                newName: "Rating",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "PayOS_Webhook_Log",
                schema: "AgroTempV3",
                newName: "PayOS_Webhook_Log",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "PayOS_Transaction",
                schema: "AgroTempV3",
                newName: "PayOS_Transaction",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "PayOS_Order_Item",
                schema: "AgroTempV3",
                newName: "PayOS_Order_Item",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "PayOS_Order",
                schema: "AgroTempV3",
                newName: "PayOS_Order",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "PayOS_Invoice",
                schema: "AgroTempV3",
                newName: "PayOS_Invoice",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Payment",
                schema: "AgroTempV3",
                newName: "Payment",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Notification",
                schema: "AgroTempV3",
                newName: "Notification",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Job_Skill_Requirement",
                schema: "AgroTempV3",
                newName: "Job_Skill_Requirement",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Job_Post",
                schema: "AgroTempV3",
                newName: "Job_Post",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Job_Detail",
                schema: "AgroTempV3",
                newName: "Job_Detail",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Job_Category",
                schema: "AgroTempV3",
                newName: "Job_Category",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "job_attachment",
                schema: "AgroTempV3",
                newName: "job_attachment",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Job_Application",
                schema: "AgroTempV3",
                newName: "Job_Application",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Farmer",
                schema: "AgroTempV3",
                newName: "Farmer",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Farm",
                schema: "AgroTempV3",
                newName: "Farm",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Dispute_Reports",
                schema: "AgroTempV3",
                newName: "Dispute_Reports",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Dispute_Report_Comments",
                schema: "AgroTempV3",
                newName: "Dispute_Report_Comments",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Device_Token",
                schema: "AgroTempV3",
                newName: "Device_Token",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Chat_Message",
                schema: "AgroTempV3",
                newName: "Chat_Message",
                newSchema: "AgroTempV2");

            migrationBuilder.RenameTable(
                name: "Blacklisted_Tokens",
                schema: "AgroTempV3",
                newName: "Blacklisted_Tokens",
                newSchema: "AgroTempV2");

            migrationBuilder.AddColumn<List<DateOnly>>(
                name: "selected_days",
                schema: "AgroTempV2",
                table: "Job_Post",
                type: "date[]",
                nullable: false);
        }
    }
}
