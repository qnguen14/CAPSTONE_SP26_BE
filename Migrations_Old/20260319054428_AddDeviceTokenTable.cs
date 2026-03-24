using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Comment out JobPost changes - đã tồn tại trong DB
            // migrationBuilder.DropColumn(
            //     name: "age_requirement",
            //     schema: "AgroTempV1",
            //     table: "Job_Post");

            // migrationBuilder.DropColumn(
            //     name: "latitude",
            //     schema: "AgroTempV1",
            //     table: "Job_Post");

            // migrationBuilder.DropColumn(
            //     name: "longitude",
            //     schema: "AgroTempV1",
            //     table: "Job_Post");

            // migrationBuilder.AddColumn<Guid>(
            //     name: "farm_id",
            //     schema: "AgroTempV1",
            //     table: "Job_Post",
            //     type: "uuid",
            //     nullable: false,
            //     defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "DeviceToken",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expo_push_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    platform = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    device_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceToken", x => x.id);
                    table.ForeignKey(
                        name: "FK_DeviceToken_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV1",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Comment out JobPost index and FK - đã tồn tại
            // migrationBuilder.CreateIndex(
            //     name: "IX_Job_Post_farm_id",
            //     schema: "AgroTempV1",
            //     table: "Job_Post",
            //     column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceToken_user_id_expo_push_token",
                schema: "AgroTempV1",
                table: "DeviceToken",
                columns: new[] { "user_id", "expo_push_token" },
                unique: true);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Job_Post_Farm_farm_id",
            //     schema: "AgroTempV1",
            //     table: "Job_Post",
            //     column: "farm_id",
            //     principalSchema: "AgroTempV1",
            //     principalTable: "Farm",
            //     principalColumn: "id",
            //     onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Comment out JobPost rollback - không cần
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Job_Post_Farm_farm_id",
            //     schema: "AgroTempV1",
            //     table: "Job_Post");

            migrationBuilder.DropTable(
                name: "DeviceToken",
                schema: "AgroTempV1");

            // migrationBuilder.DropIndex(
            //     name: "IX_Job_Post_farm_id",
            //     schema: "AgroTempV1",
            //     table: "Job_Post");

            // migrationBuilder.DropColumn(
            //     name: "farm_id",
            //     schema: "AgroTempV1",
            //     table: "Job_Post");

            // migrationBuilder.AddColumn<string>(
            //     name: "age_requirement",
            //     schema: "AgroTempV1",
            //     table: "Job_Post",
            //     type: "text",
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<decimal>(
            //     name: "latitude",
            //     schema: "AgroTempV1",
            //     table: "Job_Post",
            //     type: "numeric(10,7)",
            //     precision: 10,
            //     scale: 7,
            //     nullable: false,
            //     defaultValue: 0m);

            // migrationBuilder.AddColumn<decimal>(
            //     name: "longitude",
            //     schema: "AgroTempV1",
            //     table: "Job_Post",
            //     type: "numeric(10,7)",
            //     precision: 10,
            //     scale: 7,
            //     nullable: false,
            //     defaultValue: 0m);
        }
    }
}
