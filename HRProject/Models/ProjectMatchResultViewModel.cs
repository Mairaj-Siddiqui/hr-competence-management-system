namespace HRProject.Models
{
    public class ProjectMatchResultViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }

        public int CompetenceMatchPercent { get; set; }
        public int ExperienceMatchPercent { get; set; }
        public int AvailabilityMatchPercent { get; set; }

        public double OverallMatchPercent { get; set; }
    }
}
