using System.Collections.Generic;              // Gives us access to List<T> and other collection types
using System.ComponentModel.DataAnnotations;  // Optional: for display attributes

namespace HRProject.Models                    // Namespace of your project, same as other models
{
    /// <summary>
    /// This class represents all the data that the HR Dashboard page needs.
    /// The controller will fill this with values from the database
    /// and then pass it to the View.
    /// </summary>
    public class HRDashboardViewModel
    {
        // Total number of users in the system (all ApplicationUser records)
        [Display(Name = "Total Employees")]
        public int TotalEmployees { get; set; }

        // Total number of competences that exist in the competence catalogue
        [Display(Name = "Total Competences")]
        public int TotalCompetences { get; set; }

        // Number of users who have at least ONE competence connected to them
        [Display(Name = "Users With Competences")]
        public int UsersWithCompetences { get; set; }

        // Number of users that do NOT have any competence yet
        [Display(Name = "Users Without Any Competence")]
        public int UsersWithoutCompetences { get; set; }

        // A small list showing which competences are most common in the company
        [Display(Name = "Top Competences")]
        public List<HRDashboardCompetenceSummary> TopCompetences { get; set; }
            = new List<HRDashboardCompetenceSummary>(); // We create an empty list by default
    }

    /// <summary>
    /// This helper class represents ONE row in the "Top competences" list.
    /// Example: "C#" used by 5 people.
    /// </summary>
    public class HRDashboardCompetenceSummary
    {
        // Name of the competence (e.g. "C#", "Azure", "Communication")
        public string CompetenceName { get; set; }

        // How many different users have this competence
        public int UserCount { get; set; }
    }
}
