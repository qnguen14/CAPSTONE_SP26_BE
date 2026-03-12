using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farm_Farmer_Profile_farmer_profile_id",
                schema: "AgroTempV1",
                table: "Farm");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Application_Worker_Profile_worker_profile_id",
                schema: "AgroTempV1",
                table: "Job_Application");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Post_Farmer_Profile_farmer_profile_id",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Worker_Skill_Worker_Profile_worker_profile_id",
                schema: "AgroTempV1",
                table: "Worker_Skill");

            migrationBuilder.DropTable(
                name: "Farmer_Profile",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Job_Assignment",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Worker_Attendance",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Worker_Profile",
                schema: "AgroTempV1");

            migrationBuilder.RenameColumn(
                name: "worker_profile_id",
                schema: "AgroTempV1",
                table: "Worker_Skill",
                newName: "worker_id");

            migrationBuilder.RenameIndex(
                name: "IX_Worker_Skill_worker_profile_id",
                schema: "AgroTempV1",
                table: "Worker_Skill",
                newName: "IX_Worker_Skill_worker_id");

            migrationBuilder.RenameColumn(
                name: "farmer_profile_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "farmer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Job_Post_farmer_profile_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "IX_Job_Post_farmer_id");

            migrationBuilder.RenameColumn(
                name: "worker_profile_id",
                schema: "AgroTempV1",
                table: "Job_Application",
                newName: "worker_id");

            migrationBuilder.RenameIndex(
                name: "IX_Job_Application_worker_profile_id",
                schema: "AgroTempV1",
                table: "Job_Application",
                newName: "IX_Job_Application_worker_id");

            migrationBuilder.RenameColumn(
                name: "farmer_profile_id",
                schema: "AgroTempV1",
                table: "Farm",
                newName: "farmer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Farm_farmer_profile_id",
                schema: "AgroTempV1",
                table: "Farm",
                newName: "IX_Farm_farmer_id");

            migrationBuilder.CreateTable(
                name: "Farmer",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    contact_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    contact_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    cooperative_affiliation = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    farm_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    total_jobs_posted = table.Column<int>(type: "integer", nullable: false),
                    total_jobs_completed = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farmer", x => x.id);
                    table.ForeignKey(
                        name: "FK_Farmer_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    age_range = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    primary_location = table.Column<string>(type: "text", nullable: false),
                    travel_radius_km_preference = table.Column<double>(type: "double precision", nullable: true),
                    experience_level = table.Column<int>(type: "integer", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    availability_schedule = table.Column<string>(type: "text", nullable: false),
                    total_jobs_completed = table.Column<int>(type: "integer", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker", x => x.id);
                    table.ForeignKey(
                        name: "FK_Worker_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job_Detail",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Job_Detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_Job_Detail_Job_Application_job_application_id",
                        column: x => x.job_application_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Detail_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Detail_Worker_worker_id",
                        column: x => x.worker_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Worker",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Worker_Session",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_detail_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Worker_Session", x => x.id);
                    table.ForeignKey(
                        name: "FK_Worker_Session_Job_Detail_job_detail_id",
                        column: x => x.job_detail_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Detail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Farmer_user_id",
                schema: "AgroTempV1",
                table: "Farmer",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Job_Detail_job_application_id",
                schema: "AgroTempV1",
                table: "Job_Detail",
                column: "job_application_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Detail_job_post_id",
                schema: "AgroTempV1",
                table: "Job_Detail",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Detail_worker_id",
                schema: "AgroTempV1",
                table: "Job_Detail",
                column: "worker_id");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_user_id",
                schema: "AgroTempV1",
                table: "Worker",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Session_job_detail_id",
                schema: "AgroTempV1",
                table: "Worker_Session",
                column: "job_detail_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_Farmer_farmer_id",
                schema: "AgroTempV1",
                table: "Farm",
                column: "farmer_id",
                principalSchema: "AgroTempV1",
                principalTable: "Farmer",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Application_Worker_worker_id",
                schema: "AgroTempV1",
                table: "Job_Application",
                column: "worker_id",
                principalSchema: "AgroTempV1",
                principalTable: "Worker",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Post_Farmer_farmer_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                column: "farmer_id",
                principalSchema: "AgroTempV1",
                principalTable: "Farmer",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Worker_Skill_Worker_worker_id",
                schema: "AgroTempV1",
                table: "Worker_Skill",
                column: "worker_id",
                principalSchema: "AgroTempV1",
                principalTable: "Worker",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farm_Farmer_farmer_id",
                schema: "AgroTempV1",
                table: "Farm");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Application_Worker_worker_id",
                schema: "AgroTempV1",
                table: "Job_Application");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Post_Farmer_farmer_id",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Worker_Skill_Worker_worker_id",
                schema: "AgroTempV1",
                table: "Worker_Skill");

            migrationBuilder.DropTable(
                name: "Farmer",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Worker_Session",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Job_Detail",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Worker",
                schema: "AgroTempV1");

            migrationBuilder.RenameColumn(
                name: "worker_id",
                schema: "AgroTempV1",
                table: "Worker_Skill",
                newName: "worker_profile_id");

            migrationBuilder.RenameIndex(
                name: "IX_Worker_Skill_worker_id",
                schema: "AgroTempV1",
                table: "Worker_Skill",
                newName: "IX_Worker_Skill_worker_profile_id");

            migrationBuilder.RenameColumn(
                name: "farmer_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "farmer_profile_id");

            migrationBuilder.RenameIndex(
                name: "IX_Job_Post_farmer_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "IX_Job_Post_farmer_profile_id");

            migrationBuilder.RenameColumn(
                name: "worker_id",
                schema: "AgroTempV1",
                table: "Job_Application",
                newName: "worker_profile_id");

            migrationBuilder.RenameIndex(
                name: "IX_Job_Application_worker_id",
                schema: "AgroTempV1",
                table: "Job_Application",
                newName: "IX_Job_Application_worker_profile_id");

            migrationBuilder.RenameColumn(
                name: "farmer_id",
                schema: "AgroTempV1",
                table: "Farm",
                newName: "farmer_profile_id");

            migrationBuilder.RenameIndex(
                name: "IX_Farm_farmer_id",
                schema: "AgroTempV1",
                table: "Farm",
                newName: "IX_Farm_farmer_profile_id");

            migrationBuilder.CreateTable(
                name: "Farmer_Profile",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    contact_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    contact_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    cooperative_affiliation = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    farm_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    organization_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    total_jobs_completed = table.Column<int>(type: "integer", nullable: false),
                    total_jobs_posted = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farmer_Profile", x => x.id);
                    table.ForeignKey(
                        name: "FK_Farmer_Profile_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker_Attendance",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    check_in_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    check_in_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    check_out_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    check_out_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    total_hours_worked = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    work_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "Worker_Profile",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    age_range = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    availability_schedule = table.Column<string>(type: "text", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    experience_level = table.Column<int>(type: "integer", nullable: false),
                    full_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    primary_location = table.Column<string>(type: "text", nullable: false),
                    total_jobs_completed = table.Column<int>(type: "integer", nullable: false),
                    travel_radius_km_preference = table.Column<double>(type: "double precision", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker_Profile", x => x.id);
                    table.ForeignKey(
                        name: "FK_Worker_Profile_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    confirmed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    farmer_confirmed_attendance = table.Column<bool>(type: "boolean", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    total_amount_due = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    total_hours_worked = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    worker_checked_in = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Farmer_Profile_user_id",
                schema: "AgroTempV1",
                table: "Farmer_Profile",
                column: "user_id",
                unique: true);

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
                name: "IX_Worker_Attendance_job_application_id",
                schema: "AgroTempV1",
                table: "Worker_Attendance",
                column: "job_application_id");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Profile_user_id",
                schema: "AgroTempV1",
                table: "Worker_Profile",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Farm_Farmer_Profile_farmer_profile_id",
                schema: "AgroTempV1",
                table: "Farm",
                column: "farmer_profile_id",
                principalSchema: "AgroTempV1",
                principalTable: "Farmer_Profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Application_Worker_Profile_worker_profile_id",
                schema: "AgroTempV1",
                table: "Job_Application",
                column: "worker_profile_id",
                principalSchema: "AgroTempV1",
                principalTable: "Worker_Profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Post_Farmer_Profile_farmer_profile_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                column: "farmer_profile_id",
                principalSchema: "AgroTempV1",
                principalTable: "Farmer_Profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Worker_Skill_Worker_Profile_worker_profile_id",
                schema: "AgroTempV1",
                table: "Worker_Skill",
                column: "worker_profile_id",
                principalSchema: "AgroTempV1",
                principalTable: "Worker_Profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
