namespace HRProject.Models
{
    public class Competence
    {
        public int Id { get; set; }

        // Name of the competence/skill (e.g. "C#", "Scrum", "SQL")
        public string Name { get; set; }

        // Optional description
        public string Description { get; set; }

        // Navigation property: which users have this competence
        public ICollection<UserCompetence> UserCompetences { get; set; }
    }
}
