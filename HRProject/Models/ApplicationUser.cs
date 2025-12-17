using Microsoft.AspNetCore.Identity;

namespace HRProject.Models
{
    // We will add more properties later (skills, availability, etc.)
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? JobTitle { get; set; }

        public int AvailabilityPercent { get; set; } = 100; // 0–100, default fully available

        public string? Certificates { get; set; }      // plain text list
        public string? Languages { get; set; }        // plain text list
        public string? Interests { get; set; }        // what projects they like

        public ICollection<UserCompetence> UserCompetences { get; set; } = new List<UserCompetence>();

        public ICollection<ProjectTeamMember> ProjectTeamMemberships { get; set; }
            = new List<ProjectTeamMember>();
    }
}
