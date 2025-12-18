namespace HRProject.Models
{
    public class ProjectTeamMember
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public ProjectManager Project { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string AssignedRole { get; set; }
    }
}