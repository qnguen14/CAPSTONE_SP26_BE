using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRoleColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                schema: "AgroTempV1",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Farmer_Profile",
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
                    table.PrimaryKey("PK_Farmer_Profile", x => x.id);
                    table.ForeignKey(
                        name: "FK_Farmer_Profile_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker_Profile",
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
                    ExperienceLevel = table.Column<int>(type: "integer", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    availability_schedule = table.Column<string>(type: "text", nullable: false),
                    total_jobs_completed = table.Column<int>(type: "integer", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker_Profile", x => x.id);
                    table.ForeignKey(
                        name: "FK_Worker_Profile_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Farm",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farmer_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: false),
                    location_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farm", x => x.id);
                    table.ForeignKey(
                        name: "FK_Farm_Farmer_Profile_farmer_profile_id",
                        column: x => x.farmer_profile_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "Farmer_Profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Farm_farmer_profile_id",
                schema: "AgroTempV1",
                table: "Farm",
                column: "farmer_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_Farmer_Profile_user_id",
                schema: "AgroTempV1",
                table: "Farmer_Profile",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Profile_user_id",
                schema: "AgroTempV1",
                table: "Worker_Profile",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Farm",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Worker_Profile",
                schema: "AgroTempV1");

            migrationBuilder.DropTable(
                name: "Farmer_Profile",
                schema: "AgroTempV1");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                schema: "AgroTempV1",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
