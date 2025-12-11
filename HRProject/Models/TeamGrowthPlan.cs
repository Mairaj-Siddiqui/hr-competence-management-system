using System;
using System.ComponentModel.DataAnnotations;

namespace HRProject.Models
{
    public class TeamGrowthPlan
    {
        public int Id { get; set; }

        public int TeamLeaderId { get; set; }
        public TeamLeader TeamLeader { get; set; }

        public string Action { get; set; }
        public string Goal { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; } // NotStarted / InProgress / Done

       

    }
}
