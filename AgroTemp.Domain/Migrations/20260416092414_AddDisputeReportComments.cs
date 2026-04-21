using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDisputeReportComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dispute_Report_Comments",
                schema: "AgroTempV2",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    dispute_report_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    attachment_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispute_Report_Comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Dispute_Report_Comments_Dispute_Reports_dispute_report_id",
                        column: x => x.dispute_report_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "Dispute_Reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dispute_Report_Comments_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "AgroTempV2",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_Report_Comments_dispute_report_id",
                schema: "AgroTempV2",
                table: "Dispute_Report_Comments",
                column: "dispute_report_id");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_Report_Comments_user_id",
                schema: "AgroTempV2",
                table: "Dispute_Report_Comments",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dispute_Report_Comments",
                schema: "AgroTempV2");
        }
    }
}
