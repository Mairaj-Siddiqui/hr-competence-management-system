using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using HRProject.Data;
using HRProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    /// <summary>
    /// Controller for all HR-related dashboards, analysis and reports.
    /// </summary>
    [Authorize(Roles = "Admin,HR")] // keep enabled/disabled as needed
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HRController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        // =====================================================
        // HR DASHBOARD
        // =====================================================
        public async Task<IActionResult> Index()
        {
            var totalEmployees = _userManager.Users.Count();
            var totalCompetences = _context.Competences.Count();

            var usersWithCompetences = _context.UserCompetences
                .Select(uc => uc.UserId)
                .Distinct()
                .Count();

            var usersWithoutCompetences = totalEmployees - usersWithCompetences;

            var topCompetences = _context.UserCompetences
                .GroupBy(uc => uc.Competence.Name)
                .Select(group => new HRDashboardCompetenceSummary
                {
                    CompetenceName = group.Key,
                    UserCount = group
                        .Select(uc => uc.UserId)
                        .Distinct()
                        .Count()
                })
                .OrderByDescending(x => x.UserCount)
                .Take(5)
                .ToList();

            var viewModel = new HRDashboardViewModel
            {
                TotalEmployees = totalEmployees,
                TotalCompetences = totalCompetences,
                UsersWithCompetences = usersWithCompetences,
                UsersWithoutCompetences = usersWithoutCompetences,
                TopCompetences = topCompetences
            };

            return View(viewModel);
        }

        // =====================================================
        // USERS WITHOUT COMPETENCES
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> EmployeesWithoutCompetences()
        {
            var userIdsWithCompetences = await _context.UserCompetences
                .Select(uc => uc.UserId)
                .Distinct()
                .ToListAsync();

            var usersWithoutCompetences = await _userManager.Users
                .Where(u => !userIdsWithCompetences.Contains(u.Id))
                .OrderBy(u => u.Email)
                .ToListAsync();

            return View(usersWithoutCompetences);
        }

        // =====================================================
        // USERS BY COMPETENCE
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> CompetenceUsers(string competenceName)
        {
            if (string.IsNullOrWhiteSpace(competenceName))
                return RedirectToAction(nameof(Index));

            var usersWithCompetence = await _context.UserCompetences
                .Include(uc => uc.User)
                .Include(uc => uc.Competence)
                .Where(uc => uc.Competence.Name == competenceName)
                .OrderBy(uc => uc.User.Email)
                .ToListAsync();

            ViewBag.CompetenceName = competenceName;
            return View(usersWithCompetence);
        }

        // =====================================================
        // HR SKILL GAP – OVERVIEW (REAL DATA OR DEMO MODE)
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> SkillGap()
        {
            // DEMO MODE = if no structured project requirements exist yet
            bool useDummyData = !await _context.ProjectRequirements.AnyAsync();

            var rows = new List<HRSkillGapRowViewModel>();

            int totalProjectRequirements = 0;
            int uniqueRequiredCompetences = 0;

            if (useDummyData)
            {
                // -------------------------------------------------
                // DUMMY DATA (matches your charts + table)
                // IMPORTANT: This is ONLY for presentation/demo.
                // -------------------------------------------------
                rows = new List<HRSkillGapRowViewModel>
                {
                    new HRSkillGapRowViewModel
                    {
                        CompetenceId = 1,
                        CompetenceName = "C#",
                        ProjectsRequiring = 4,
                        MaxMinLevelRequired = (int)CompetenceLevel.Intermediate,
                        MaxMinYearsRequired = 2,
                        EmployeesAvailable = 2,
                        Status = "GAP"
                    },
                    new HRSkillGapRowViewModel
                    {
                        CompetenceId = 2,
                        CompetenceName = "Azure",
                        ProjectsRequiring = 3,
                        MaxMinLevelRequired = (int)CompetenceLevel.Intermediate,
                        MaxMinYearsRequired = 2,
                        EmployeesAvailable = 4,
                        Status = "OK"
                    },
                    new HRSkillGapRowViewModel
                    {
                        CompetenceId = 3,
                        CompetenceName = "SQL",
                        ProjectsRequiring = 2,
                        MaxMinLevelRequired = (int)CompetenceLevel.Basic,
                        MaxMinYearsRequired = 1,
                        EmployeesAvailable = 1,
                        Status = "GAP"
                    },
                    new HRSkillGapRowViewModel
                    {
                        CompetenceId = 4,
                        CompetenceName = "React",
                        ProjectsRequiring = 5,
                        MaxMinLevelRequired = (int)CompetenceLevel.Basic,
                        MaxMinYearsRequired = 1,
                        EmployeesAvailable = 3,
                        Status = "GAP"
                    }
                };

                // KPI values from dummy rows
                totalProjectRequirements = rows.Sum(r => r.ProjectsRequiring);
                uniqueRequiredCompetences = rows.Count;
            }
            else
            {
                // -------------------------------------------------
                // REAL DATA (existing logic)
                // -------------------------------------------------
                var demand = await _context.ProjectRequirements
                    .GroupBy(r => r.CompetenceId)
                    .Select(g => new
                    {
                        CompetenceId = g.Key,
                        ProjectsRequiring = g.Select(x => x.ProjectId).Distinct().Count(),
                        MaxMinLevel = (int)g.Max(x => x.MinLevel),
                        MaxMinYears = g.Max(x => x.MinYearsOfExperience)
                    })
                    .ToListAsync();

                var supply = await _context.UserCompetences
                    .GroupBy(uc => uc.CompetenceId)
                    .Select(g => new
                    {
                        CompetenceId = g.Key,
                        EmployeesAvailable = g.Select(x => x.UserId).Distinct().Count()
                    })
                    .ToListAsync();

                var competences = await _context.Competences
                    .Select(c => new { c.Id, c.Name })
                    .ToListAsync();

                var supplyDict = supply.ToDictionary(x => x.CompetenceId, x => x.EmployeesAvailable);

                foreach (var d in demand)
                {
                    var comp = competences.FirstOrDefault(c => c.Id == d.CompetenceId);
                    var competenceName = comp?.Name ?? $"CompetenceId {d.CompetenceId}";

                    var employeesAvailable = supplyDict.ContainsKey(d.CompetenceId)
                        ? supplyDict[d.CompetenceId]
                        : 0;

                    string status =
                        d.ProjectsRequiring > employeesAvailable ? "GAP" :
                        d.ProjectsRequiring == employeesAvailable ? "TIGHT" :
                        "OK";

                    rows.Add(new HRSkillGapRowViewModel
                    {
                        CompetenceId = d.CompetenceId,
                        CompetenceName = competenceName,
                        ProjectsRequiring = d.ProjectsRequiring,
                        MaxMinLevelRequired = d.MaxMinLevel,
                        MaxMinYearsRequired = d.MaxMinYears,
                        EmployeesAvailable = employeesAvailable,
                        Status = status
                    });
                }

                totalProjectRequirements = await _context.ProjectRequirements.CountAsync();
                uniqueRequiredCompetences = demand.Count;
            }

            // KPI Cards (works both for dummy and real data)
            var competencesWithGaps = rows.Count(r => r.Status == "GAP");
            var criticalCompetences = rows.Count(r => r.EmployeesAvailable <= 1 && r.ProjectsRequiring > 0);

            var vm = new HRSkillGapOverviewViewModel
            {
                // NOTE: These now match dummy mode AND real mode
                TotalProjectRequirements = totalProjectRequirements,
                UniqueRequiredCompetences = uniqueRequiredCompetences,
                CompetencesWithGaps = competencesWithGaps,
                CriticalCompetences = criticalCompetences,

                // Recommended: add this bool to ViewModel (Step 2 if not exists)
                UseDummyData = useDummyData,

                Rows = rows
                    .OrderByDescending(r => r.Status == "GAP")
                    .ThenByDescending(r => r.ProjectsRequiring)
                    .ThenBy(r => r.CompetenceName)
                    .ToList()
            };

            return View(vm);
        }

        // =====================================================
        // HR SKILL GAP – DETAILS
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> SkillGapDetails(int competenceId)
        {
            var competence = await _context.Competences.FirstOrDefaultAsync(c => c.Id == competenceId);
            if (competence == null)
                return RedirectToAction(nameof(SkillGap));

            var projectDemand = await _context.ProjectRequirements
                .Include(r => r.Project)
                .Where(r => r.CompetenceId == competenceId)
                .GroupBy(r => new { r.ProjectId, ProjectName = r.Project.Name })
                .Select(g => new HRSkillGapProjectDemandRow
                {
                    ProjectId = g.Key.ProjectId,
                    ProjectName = g.Key.ProjectName ?? $"ProjectId {g.Key.ProjectId}",
                    MinLevel = (int)g.Max(x => x.MinLevel),
                    MinYears = g.Max(x => x.MinYearsOfExperience)
                })
                .ToListAsync();

            var employeeSupply = await _context.UserCompetences
                .Include(uc => uc.User)
                .Where(uc => uc.CompetenceId == competenceId)
                .OrderByDescending(uc => uc.Level)
                .ThenByDescending(uc => uc.YearsOfExperience)
                .Select(uc => new HRSkillGapEmployeeSupplyRow
                {
                    FullName = uc.User.FullName ?? "",
                    Email = uc.User.Email ?? "",
                    JobTitle = uc.User.JobTitle ?? "",
                    AvailabilityPercent = uc.User.AvailabilityPercent,
                    Level = uc.Level,
                    YearsOfExperience = uc.YearsOfExperience
                })
                .ToListAsync();

            var vm = new HRSkillGapDetailsViewModel
            {
                CompetenceId = competence.Id,
                CompetenceName = competence.Name ?? "",
                ProjectDemand = projectDemand,
                EmployeeSupply = employeeSupply,
                ProjectsRequiring = projectDemand.Count,
                EmployeesAvailable = employeeSupply.Count,
                SummaryMessage = $"'{competence.Name}' required in {projectDemand.Count} project(s), " +
                                 $"{employeeSupply.Count} employee(s) available."
            };

            return View(vm);
        }

        // =====================================================
        // HR REPORTS – BAR CHART + HEATMAP
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            var competenceDistribution = await _context.UserCompetences
                .Include(uc => uc.Competence)
                .GroupBy(uc => uc.Competence.Name)
                .Select(g => new
                {
                    Competence = g.Key,
                    Users = g.Select(x => x.UserId).Distinct().Count()
                })
                .OrderByDescending(x => x.Users)
                .ToListAsync();

            var users = await _context.Users.OrderBy(u => u.FullName).ToListAsync();
            var competences = await _context.Competences.OrderBy(c => c.Name).ToListAsync();
            var userCompetences = await _context.UserCompetences.ToListAsync();

            var matrix = new List<List<int>>();

            foreach (var user in users)
            {
                var row = new List<int>();

                foreach (var comp in competences)
                {
                    var match = userCompetences
                        .FirstOrDefault(uc => uc.UserId == user.Id && uc.CompetenceId == comp.Id);

                    row.Add(match == null ? 0 : (int)match.Level);
                }

                matrix.Add(row);
            }

            var vm = new HRChartsViewModel
            {
                CompetenceLabels = competenceDistribution.Select(x => x.Competence).ToList(),
                CompetenceCounts = competenceDistribution.Select(x => x.Users).ToList(),
                EmployeeLabels = users.Select(u => u.FullName ?? u.Email).ToList(),
                HeatmapCompetenceLabels = competences.Select(c => c.Name).ToList(),
                HeatmapMatrix = matrix
            };

            return View(vm);
        }

        // =====================================================
        // EXPORT – TEAM SKILLS OVERVIEW (CSV / Excel)
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> ExportTeamSkillsCsv()
        {
            var data = await _context.UserCompetences
                .Include(uc => uc.User)
                .Include(uc => uc.Competence)
                .OrderBy(uc => uc.User.Email)
                .ThenBy(uc => uc.Competence.Name)
                .ToListAsync();

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("FullName;Email;JobTitle;AvailabilityPercent;Competence;Level;YearsOfExperience");

            foreach (var row in data)
            {
                var fullName = EscapeCsv(row.User?.FullName);
                var email = EscapeCsv(row.User?.Email);
                var jobTitle = EscapeCsv(row.User?.JobTitle);
                var availability = row.User?.AvailabilityPercent ?? 0;

                var competence = EscapeCsv(row.Competence?.Name);
                var level = row.Level.ToString();
                var years = row.YearsOfExperience?.ToString() ?? "";

                csv.AppendLine($"{fullName};{email};{jobTitle};{availability};{competence};{level};{years}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "HR_TeamSkills.csv");
        }

        // Helper for CSV formatting
        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            value = value.Replace("\"", "\"\"");

            // IMPORTANT: also handle semicolon since we use ; as delimiter
            if (value.Contains(";") || value.Contains(",") || value.Contains("\n") || value.Contains("\r"))
                return $"\"{value}\"";

            return value;
        }
    }
}
