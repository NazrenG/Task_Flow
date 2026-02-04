using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForCanban : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CanbanColumns_Projects_ProjectId",
                table: "CanbanColumns");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "CanbanColumns",
                newName: "SprintId");

            migrationBuilder.RenameIndex(
                name: "IX_CanbanColumns_ProjectId",
                table: "CanbanColumns",
                newName: "IX_CanbanColumns_SprintId");

            migrationBuilder.AddForeignKey(
                name: "FK_CanbanColumns_Sprints_SprintId",
                table: "CanbanColumns",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CanbanColumns_Sprints_SprintId",
                table: "CanbanColumns");

            migrationBuilder.RenameColumn(
                name: "SprintId",
                table: "CanbanColumns",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_CanbanColumns_SprintId",
                table: "CanbanColumns",
                newName: "IX_CanbanColumns_ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_CanbanColumns_Projects_ProjectId",
                table: "CanbanColumns",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
