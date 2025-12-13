using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompetenceWeight = table.Column<int>(type: "int", nullable: false),
                    ExperienceWeight = table.Column<int>(type: "int", nullable: false),
                    AvailabilityWeight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchSettings");
        }
    }
}
