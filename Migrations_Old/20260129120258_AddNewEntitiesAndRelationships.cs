using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddNewEntitiesAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chat_Message",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_content = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat_Message", x => x.id);
                    table.ForeignKey(
                        name: "FK_Chat_Message_User_recipient_id",
                        column: x => x.recipient_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chat_Message_User_sender_id",
                        column: x => x.sender_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Job_Application",
                schema: "AgroTempV1",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    cover_letter = table.Column<string>(type: "text", nullable: true),
                    applied_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    response_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Application", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_Application_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job_Application_Worker_Profile_worker_profile_id",
                        column: x => x.worker_profile_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Worker_Profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.id);
                    table.ForeignKey(
                        name: "FK_Notification_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Job_Assignment",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    confirmed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    worker_checked_in = table.Column<bool>(type: "boolean", nullable: false),
                    farmer_confirmed_attendance = table.Column<bool>(type: "boolean", nullable: false),
                    total_hours_worked = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    total_amount_due = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Assignment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Job_Assignment_Job_Application_job_application_id",
                        column: x => x.job_application_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Assignment_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Assignment_Worker_Profile_worker_profile_id",
                        column: x => x.worker_profile_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Worker_Profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Worker_Attendance",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    check_in_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    check_in_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    check_out_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    check_out_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    total_hours_worked = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker_Attendance", x => x.id);
                    table.ForeignKey(
                        name: "FK_Worker_Attendance_Job_Application_job_application_id",
                        column: x => x.job_application_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job_Skill_Requirement",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    required_level = table.Column<int>(type: "integer", nullable: false),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Skill_Requirement", x => x.id);
                    table.ForeignKey(
                        name: "FK_Job_Skill_Requirement_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job_Skill_Requirement_Skill_skill_id",
                        column: x => x.skill_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Skill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Worker_Skill",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_level = table.Column<int>(type: "integer", nullable: false),
                    years_experience = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker_Skill", x => x.id);
                    table.ForeignKey(
                        name: "FK_Worker_Skill_Skill_skill_id",
                        column: x => x.skill_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Skill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Worker_Skill_Worker_Profile_worker_profile_id",
                        column: x => x.worker_profile_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Worker_Profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Message_recipient_id",
                schema: "AgroTempV1",
                table: "Chat_Message",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Message_sender_id",
                schema: "AgroTempV1",
                table: "Chat_Message",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Application_job_post_id",
                schema: "AgroTempV1",
                table: "Job_Application",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Application_worker_profile_id",
                schema: "AgroTempV1",
                table: "Job_Application",
                column: "worker_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Assignment_job_application_id",
                schema: "AgroTempV1",
                table: "Job_Assignment",
                column: "job_application_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Assignment_job_post_id",
                schema: "AgroTempV1",
                table: "Job_Assignment",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Assignment_worker_profile_id",
                schema: "AgroTempV1",
                table: "Job_Assignment",
                column: "worker_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Skill_Requirement_job_post_id",
                schema: "AgroTempV1",
                table: "Job_Skill_Requirement",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Skill_Requirement_skill_id",
                schema: "AgroTempV1",
                table: "Job_Skill_Requirement",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_user_id",
                schema: "AgroTempV1",
                table: "Notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Attendance_job_application_id",
                schema: "AgroTempV1",
                table: "Worker_Attendance",
                column: "job_application_id");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Skill_skill_id",
                schema: "AgroTempV1",
                table: "Worker_Skill",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Skill_worker_profile_id",
                schema: "AgroTempV1",
                table: "Worker_Skill",
                column: "worker_profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chat_Message",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Job_Assignment",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Job_Skill_Requirement",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Worker_Attendance",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Worker_Skill",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Job_Application",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Skill",
                schema: "AgroTempV1");
        }
    }
}
