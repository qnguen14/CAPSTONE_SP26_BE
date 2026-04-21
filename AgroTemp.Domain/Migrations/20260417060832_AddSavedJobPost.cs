using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedJobPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Saved_Job_Post",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saved_Job_Post", x => x.id);
                    table.ForeignKey(
                        name: "FK_Saved_Job_Post_Job_Post_job_post_id",
                        column: x => x.job_post_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Job_Post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Saved_Job_Post_Worker_worker_id",
                        column: x => x.worker_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Worker",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Saved_Job_Post_job_post_id",
                schema: "AgroTempV2",
                table: "Saved_Job_Post",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Saved_Job_Post_worker_id_job_post_id",
                schema: "AgroTempV2",
                table: "Saved_Job_Post",
                columns: new[] { "worker_id", "job_post_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Saved_Job_Post",
                schema: "AgroTempV2");
        }
    }
}
