using System.ComponentModel.DataAnnotations;

namespace HRProject.Models
{
    public class MatchSettings
    {
        public int Id { get; set; }   // primary key

        [Range(0, 100)]
        [Display(Name = "Competence weight (%)")]
        public int CompetenceWeight { get; set; } = 50;

        [Range(0, 100)]
        [Display(Name = "Experience weight (%)")]
        public int ExperienceWeight { get; set; } = 30;

        [Range(0, 100)]
        [Display(Name = "Availability weight (%)")]
        public int AvailabilityWeight { get; set; } = 20;
    }
}
