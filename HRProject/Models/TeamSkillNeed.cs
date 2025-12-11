namespace HRProject.Models
{
    public class TeamSkillNeed
    {
        public int Id { get; set; }

        public int TeamLeaderId { get; set; }
        public TeamLeader TeamLeader { get; set; }

        public int CompetenceId { get; set; }
        public Competence Competence { get; set; }

        public int LevelNeeded { get; set; }  // ex: 1 = Basic, 2 = Intermediate, 3 = Advanced
        public string Importance { get; set; } // Low / Medium / High
    }
}
