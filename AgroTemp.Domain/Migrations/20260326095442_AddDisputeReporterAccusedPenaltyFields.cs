using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDisputeReporterAccusedPenaltyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "accused_user_id",
                schema: "AgroTempV2",
                table: "Dispute_Reports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "penalty_target",
                schema: "AgroTempV2",
                table: "Dispute_Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "reporter_user_id",
                schema: "AgroTempV2",
                table: "Dispute_Reports",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accused_user_id",
                schema: "AgroTempV2",
                table: "Dispute_Reports");

            migrationBuilder.DropColumn(
                name: "penalty_target",
                schema: "AgroTempV2",
                table: "Dispute_Reports");

            migrationBuilder.DropColumn(
                name: "reporter_user_id",
                schema: "AgroTempV2",
                table: "Dispute_Reports");
        }
    }
}
