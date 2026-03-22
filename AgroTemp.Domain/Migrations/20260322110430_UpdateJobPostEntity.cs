using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJobPostEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment_method",
                schema: "AgroTempV1",
                table: "Job_Post");

            migrationBuilder.RenameColumn(
                name: "wage_type",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "job_type");

            migrationBuilder.RenameColumn(
                name: "gender_preference",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "preferences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "preferences",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "gender_preference");

            migrationBuilder.RenameColumn(
                name: "job_type",
                schema: "AgroTempV1",
                table: "Job_Post",
                newName: "wage_type");

            migrationBuilder.AddColumn<int>(
                name: "payment_method",
                schema: "AgroTempV1",
                table: "Job_Post",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
