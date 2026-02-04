using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class CompanyWorkerUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                table: "CompanyWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "CompanyWorkers");
        }
    }
}
