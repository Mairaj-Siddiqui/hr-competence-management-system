using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamCapacityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredDays",
                table: "TeamLeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequiredHours",
                table: "TeamLeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequiredMonths",
                table: "TeamLeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredDays",
                table: "TeamLeaders");

            migrationBuilder.DropColumn(
                name: "RequiredHours",
                table: "TeamLeaders");

            migrationBuilder.DropColumn(
                name: "RequiredMonths",
                table: "TeamLeaders");
        }
    }
}
