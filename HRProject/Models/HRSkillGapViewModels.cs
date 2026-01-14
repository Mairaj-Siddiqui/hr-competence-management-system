using System.Collections.Generic;

namespace HRProject.Models
{
    // ====== SCREEN 1: OVERVIEW ======

    public class HRSkillGapOverviewViewModel
    {
        // KPI cards
        public int TotalProjectRequirements { get; set; }
        public int UniqueRequiredCompetences { get; set; }
        public int CompetencesWithGaps { get; set; }
        public int CriticalCompetences { get; set; } // only 0-1 employees available

        // Main table rows
        public List<HRSkillGapRowViewModel> Rows { get; set; } = new();
    }

    public class HRSkillGapRowViewModel
    {
        public int CompetenceId { get; set; }
        public string CompetenceName { get; set; } = string.Empty;

        // Demand (projects)
        public int ProjectsRequiring { get; set; }          // how many projects require this competence
        public int MaxMinLevelRequired { get; set; }        // max required min level among projects (1-3)
        public int MaxMinYearsRequired { get; set; }        // max years required among projects

        // Supply (employees)
        public int EmployeesAvailable { get; set; }         // how many employees have this competence

        // Status label for UI
        public string Status { get; set; } = "OK";          // "GAP", "TIGHT", "OK"
    }

    // ====== SCREEN 2: DETAILS ======

    public class HRSkillGapDetailsViewModel
    {
        public int CompetenceId { get; set; }
        public string CompetenceName { get; set; } = string.Empty;

        public List<HRSkillGapProjectDemandRow> ProjectDemand { get; set; } = new();
        public List<HRSkillGapEmployeeSupplyRow> EmployeeSupply { get; set; } = new();

        // helpful summary text
        public int ProjectsRequiring { get; set; }
        public int EmployeesAvailable { get; set; }
        public string SummaryMessage { get; set; } = string.Empty;
    }

    public class HRSkillGapProjectDemandRow
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int MinLevel { get; set; }
        public int MinYears { get; set; }
    }

    public class HRSkillGapEmployeeSupplyRow
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;

        public int AvailabilityPercent { get; set; }

        public CompetenceLevel Level { get; set; }
        public int? YearsOfExperience { get; set; }
    }
}
