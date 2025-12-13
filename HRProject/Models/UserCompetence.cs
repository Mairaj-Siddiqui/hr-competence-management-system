namespace HRProject.Models
{
    public class UserCompetence
    {
        // Composite key: UserId + CompetenceId (we'll configure in DbContext)

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int CompetenceId { get; set; }
        public Competence Competence { get; set; }

        public CompetenceLevel Level { get; set; }

        public int? YearsOfExperience { get; set; }


    }
}
