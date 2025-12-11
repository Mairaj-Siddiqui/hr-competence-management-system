namespace HRProject.Models
{
    public class TeamMember
    {
        public int Id { get; set; }

        public int TeamLeaderId { get; set; }
        public TeamLeader TeamLeader { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
