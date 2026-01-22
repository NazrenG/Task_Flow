using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CanbanColumnId",
                table: "Works",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Works",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SprintId",
                table: "Works",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CanbanColumns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    StatusKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsFixed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanbanColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanbanColumns_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sprints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sprints_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Works_CanbanColumnId",
                table: "Works",
                column: "CanbanColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_Works_SprintId",
                table: "Works",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_CanbanColumns_ProjectId",
                table: "CanbanColumns",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProjectId",
                table: "Sprints",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_CanbanColumns_CanbanColumnId",
                table: "Works",
                column: "CanbanColumnId",
                principalTable: "CanbanColumns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Sprints_SprintId",
                table: "Works",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_CanbanColumns_CanbanColumnId",
                table: "Works");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_Sprints_SprintId",
                table: "Works");

            migrationBuilder.DropTable(
                name: "CanbanColumns");

            migrationBuilder.DropTable(
                name: "Sprints");

            migrationBuilder.DropIndex(
                name: "IX_Works_CanbanColumnId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Works_SprintId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "CanbanColumnId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "SprintId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserTasks");
        }
    }
}
