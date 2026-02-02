using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Flow.Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForGithubIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitHubBranchName",
                table: "Works",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GitHubAccessGranted",
                table: "TeamMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "GitHubRepositoryName",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubRepositoryUrl",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubAccessToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubUsername",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubBranchName",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "GitHubAccessGranted",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "GitHubRepositoryName",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "GitHubRepositoryUrl",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "GitHubAccessToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GitHubUsername",
                table: "AspNetUsers");
        }
    }
}
