using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddPayOSWebhookLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayOS_Webhook_Log",
                schema: "AgroTempV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_code = table.Column<long>(type: "bigint", nullable: true),
                    reference = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    raw_payload = table.Column<string>(type: "text", nullable: false),
                    is_payload_parsed = table.Column<bool>(type: "boolean", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    received_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOS_Webhook_Log", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Webhook_Log_order_code",
                schema: "AgroTempV1",
                table: "PayOS_Webhook_Log",
                column: "order_code");

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Webhook_Log_received_at",
                schema: "AgroTempV1",
                table: "PayOS_Webhook_Log",
                column: "received_at");

            migrationBuilder.CreateIndex(
                name: "IX_PayOS_Webhook_Log_reference",
                schema: "AgroTempV1",
                table: "PayOS_Webhook_Log",
                column: "reference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayOS_Webhook_Log",
                schema: "AgroTempV1");
        }
    }
}
