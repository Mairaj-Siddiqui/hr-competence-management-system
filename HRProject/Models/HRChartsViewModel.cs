using System.Collections.Generic;

namespace HRProject.Models
{
    public class HRChartsViewModel
    {
        // Bar chart
        public List<string> CompetenceLabels { get; set; } = new();
        public List<int> CompetenceCounts { get; set; } = new();

        // Heatmap
        public List<string> EmployeeLabels { get; set; } = new();
        public List<string> HeatmapCompetenceLabels { get; set; } = new();

        // Heatmap values (0/1/2/3)
        // Each row = one employee, each column = one competence
        public List<List<int>> HeatmapMatrix { get; set; } = new();
    }
}
