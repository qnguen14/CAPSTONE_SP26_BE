using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDisputeReportEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dispute_reports",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farmer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    worker_id = table.Column<Guid>(type: "uuid", nullable: true),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dispute_type = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    evidence_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    admin_note = table.Column<string>(type: "text", nullable: true),
                    resolved_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispute_reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispute_reports_Farmer_farmer_id",
                        column: x => x.farmer_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Farmer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dispute_reports_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dispute_reports_User_resolved_by_id",
                        column: x => x.resolved_by_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dispute_reports_Worker_worker_id",
                        column: x => x.worker_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Worker",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dispute_reports_farmer_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "farmer_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispute_reports_job_post_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispute_reports_resolved_by_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "resolved_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispute_reports_worker_id",
                schema: "AgroTempV1",
                table: "dispute_reports",
                column: "worker_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dispute_reports",
                schema: "AgroTempV1");
        }
    }
}
