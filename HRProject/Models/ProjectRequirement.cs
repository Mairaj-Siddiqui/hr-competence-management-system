using System.ComponentModel.DataAnnotations;

namespace HRProject.Models
{
    public class ProjectRequirement
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Required]
        public int CompetenceId { get; set; }
        public Competence Competence { get; set; }

        // Minimum level required (1=Basic, 2=Intermediate, 3=Advanced)
        [Range(1, 3)]
        public int MinLevel { get; set; }

        // Minimum years of experience
        [Range(0, 50)]
        public int MinYearsOfExperience { get; set; }
    }
}
