using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddJobCategoryAndJobPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Job_Category",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Job_Post",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farmer_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estimated_hours = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    workers_needed = table.Column<int>(type: "integer", nullable: false),
                    workers_accepted = table.Column<int>(type: "integer", nullable: false),
                    wage_type = table.Column<int>(type: "integer", nullable: false),
                    WageType = table.Column<int>(type: "integer", nullable: false),
                    wage_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    payment_method = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    required_skills = table.Column<string>(type: "text", nullable: false),
                    gender_preference = table.Column<string>(type: "text", nullable: false),
                    age_requirement = table.Column<string>(type: "text", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_urgent = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job_Post", x => x.id);
                    table.ForeignKey(
                        name: "FK_Job_Post_Farmer_Profile_farmer_profile_id",
                        column: x => x.farmer_profile_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Farmer_Profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job_Post_Job_Category_job_category_id",
                        column: x => x.job_category_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Job_Category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Job_Post_farmer_profile_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                column: "farmer_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Post_job_category_id",
                schema: "AgroTempV1",
                table: "Job_Post",
                column: "job_category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Job_Post",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Job_Category",
                schema: "AgroTempV1");
        }
    }
}
