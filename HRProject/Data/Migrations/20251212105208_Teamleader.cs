using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class Teamleader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamSkillNeeds_TeamGrowthPlans_CompetenceId",
                table: "TeamSkillNeeds");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCompetences_TeamGrowthPlans_CompetenceId",
                table: "UserCompetences");

            migrationBuilder.DropTable(
                name: "TeamGrowthPlan");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TeamGrowthPlans",
                newName: "Action");

            migrationBuilder.AlterColumn<string>(
                name: "Importance",
                table: "TeamSkillNeeds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "Competences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competences", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TeamGrowthPlans_TeamLeaders_TeamLeaderId",
                table: "TeamGrowthPlans",
                column: "TeamLeaderId",
                principalTable: "TeamLeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamSkillNeeds_Competences_CompetenceId",
                table: "TeamSkillNeeds",
                column: "CompetenceId",
                principalTable: "Competences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompetences_Competences_CompetenceId",
                table: "UserCompetences",
                column: "CompetenceId",
                principalTable: "Competences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamGrowthPlans_TeamLeaders_TeamLeaderId",
                table: "TeamGrowthPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamSkillNeeds_Competences_CompetenceId",
                table: "TeamSkillNeeds");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCompetences_Competences_CompetenceId",
                table: "UserCompetences");

            migrationBuilder.DropTable(
                name: "Competences");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "TeamGrowthPlans",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Importance",
                table: "TeamSkillNeeds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "TeamGrowthPlan",
                columns: table => new
                {
                    TeamLeaderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_TeamGrowthPlan_TeamLeaders_TeamLeaderId",
                        column: x => x.TeamLeaderId,
                        principalTable: "TeamLeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TeamSkillNeeds_TeamGrowthPlans_CompetenceId",
                table: "TeamSkillNeeds",
                column: "CompetenceId",
                principalTable: "TeamGrowthPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompetences_TeamGrowthPlans_CompetenceId",
                table: "UserCompetences",
                column: "CompetenceId",
                principalTable: "TeamGrowthPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
