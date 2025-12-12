using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRProject.Models
{
    public class TeamLeader
    {
        public int Id { get; set; }

        [Required]
        public string TeamName { get; set; }

        [Required]   
        public string Description { get; set; }

        public string LeaderUserId { get; set; }
        public ApplicationUser LeaderUser { get; set; }

        public int RequiredCapacity { get; set; }

        public ICollection<TeamMember> Members { get; set; }
        public ICollection<TeamSkillNeed> SkillNeeds { get; set; }
        public ICollection<TeamGrowthPlan> GrowthPlans { get; set; }

        public int? RequiredHours { get; set; }
        public int? RequiredDays { get; set; }
        public int? RequiredMonths { get; set; }

    }
}
