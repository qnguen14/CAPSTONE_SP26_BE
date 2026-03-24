using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FixJobDetailSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "total_amount_due",
                schema: "AgroTempV1",
                table: "Job_Detail",
                newName: "job_price");

            migrationBuilder.DropColumn(
                name: "confirmed_at",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.DropColumn(
                name: "farmer_confirmed_attendance",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.DropColumn(
                name: "started_at",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.DropColumn(
                name: "total_hours_worked",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.DropColumn(
                name: "worker_checked_in",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.AddColumn<int>(
                name: "farmer_approved_percent",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "farmer_feedback",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "refund_amount",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "work_date",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "worker_description",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "worker_payment_amount",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"AgroTempV1\".\"Job_Detail\" SET job_price = 0 WHERE job_price IS NULL;");

            migrationBuilder.AlterColumn<decimal>(
                name: "job_price",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "farmer_approved_percent",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.DropColumn(
                name: "farmer_feedback",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.DropColumn(
                name: "refund_amount",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.DropColumn(
                name: "work_date",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.DropColumn(
                name: "worker_description",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.DropColumn(
                name: "worker_payment_amount",
                schema: "AgroTempV1",
                table: "Job_Detail");

            migrationBuilder.AddColumn<DateTime>(
                name: "confirmed_at",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "farmer_confirmed_attendance",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "started_at",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_hours_worked",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "worker_checked_in",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "job_price",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.RenameColumn(
                name: "job_price",
                schema: "AgroTempV1",
                table: "Job_Detail",
                newName: "total_amount_due");

            migrationBuilder.Sql("UPDATE \"AgroTempV1\".\"Job_Detail\" SET updated_at = created_at WHERE updated_at IS NULL;");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                schema: "AgroTempV1",
                table: "Job_Detail",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
