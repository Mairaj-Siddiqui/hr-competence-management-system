namespace HRProject.Models
{
    public class ProjectRole
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public ProjectManager Project { get; set; }

        public string RoleName { get; set; }
        public int RequiredCount { get; set; }
    }
}