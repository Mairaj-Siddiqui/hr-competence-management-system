using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredWorkingStyle",
                table: "ProjectManager");

            migrationBuilder.DropColumn(
                name: "RequiredExperienceLevel",
                table: "ProjectManager");

            migrationBuilder.AddColumn<string>(
                name: "ExperienceLevel",
                table: "ProjectManager",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RolesNeeded",
                table: "ProjectManager",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "ProjectManager",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceLevel",
                table: "ProjectManager");

            migrationBuilder.DropColumn(
                name: "RolesNeeded",
                table: "ProjectManager");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "ProjectManager");

            migrationBuilder.AddColumn<string>(
                name: "PreferredWorkingStyle",
                table: "ProjectManager",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequiredExperienceLevel",
                table: "ProjectManager",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
