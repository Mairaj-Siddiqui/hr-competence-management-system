using System.Linq;                                   // Gives us LINQ methods like Count(), GroupBy(), Select()
using System.Threading.Tasks;                       // Needed for async/await and Task<IActionResult>
using HRProject.Data;                               // To access ApplicationDbContext (your EF Core DbContext)
using HRProject.Models;                             // To use ApplicationUser and HRDashboardViewModel
using Microsoft.AspNetCore.Authorization;           // For the [Authorize] attribute
using Microsoft.AspNetCore.Identity;                // For UserManager<ApplicationUser>
using Microsoft.AspNetCore.Mvc;                     // For Controller and IActionResult
using Microsoft.EntityFrameworkCore;                // For ToListAsync()

namespace HRProject.Controllers                      // Same namespace pattern as other controllers
{
    /// <summary>
    /// This controller contains actions that are intended for the HR role.
    /// </summary>
    ///[Authorize(Roles = "Admin,HR")]                 // Keep commented for now (as you requested)
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

        /// <summary>
        /// Main HR Dashboard page with KPI stats.
        /// </summary>
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

        /// <summary>
        /// HR view: list all employees who have ZERO competences assigned.
        /// This supports the "Users Without Any Competence" KPI card click.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EmployeesWithoutCompetences()
        {
            // 1) Get all user IDs that already have at least one competence
            var userIdsWithCompetences = await _context.UserCompetences
                .Select(uc => uc.UserId)
                .Distinct()
                .ToListAsync();

            // 2) Get users where their Id is NOT in that list
            var usersWithoutCompetences = await _userManager.Users
                .Where(u => !userIdsWithCompetences.Contains(u.Id))
                .OrderBy(u => u.Email)
                .ToListAsync();

            // 3) Send the list to the view
            return View(usersWithoutCompetences);
        }

        /// <summary>
        /// Shows all users who have a specific competence (clicked from HR Dashboard).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CompetenceUsers(string competenceName)
        {
            if (string.IsNullOrWhiteSpace(competenceName))
            {
                return RedirectToAction(nameof(Index));
            }

            var usersWithCompetence = await _context.UserCompetences
                .Include(uc => uc.User)
                .Include(uc => uc.Competence)
                .Where(uc => uc.Competence.Name == competenceName)
                .OrderBy(uc => uc.User.Email)
                .ToListAsync();

            ViewBag.CompetenceName = competenceName;

            return View(usersWithCompetence);
        }

        // ===============================
        // HR Skill Gap - Overview Screen
        // ===============================
        [HttpGet]
        public async Task<IActionResult> SkillGap()
        {
            // 1) Demand: group project requirements by competence
            var demand = await _context.ProjectRequirements
                .GroupBy(r => r.CompetenceId)
                .Select(g => new
                {
                    CompetenceId = g.Key,
                    ProjectsRequiring = g.Select(x => x.ProjectId).Distinct().Count(),
                    MaxMinLevel = g.Max(x => x.MinLevel),
                    MaxMinYears = g.Max(x => x.MinYearsOfExperience)
                })
                .ToListAsync();

            // 2) Supply: group user competences by competence
            var supply = await _context.UserCompetences
                .GroupBy(uc => uc.CompetenceId)
                .Select(g => new
                {
                    CompetenceId = g.Key,
                    EmployeesAvailable = g.Select(x => x.UserId).Distinct().Count()
                })
                .ToListAsync();

            // 3) Join with competence catalog for names
            var competences = await _context.Competences
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            // Build quick lookup dictionaries
            var supplyDict = supply.ToDictionary(x => x.CompetenceId, x => x.EmployeesAvailable);

            var rows = new List<HRSkillGapRowViewModel>();

            foreach (var d in demand)
            {
                var comp = competences.FirstOrDefault(c => c.Id == d.CompetenceId);
                var competenceName = comp?.Name ?? $"CompetenceId {d.CompetenceId}";

                var employeesAvailable = supplyDict.ContainsKey(d.CompetenceId)
                    ? supplyDict[d.CompetenceId]
                    : 0;

                string status;
                if (d.ProjectsRequiring > employeesAvailable) status = "GAP";
                else if (d.ProjectsRequiring == employeesAvailable) status = "TIGHT";
                else status = "OK";

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

            // KPI Cards
            var totalProjectRequirements = await _context.ProjectRequirements.CountAsync();
            var uniqueRequiredCompetences = demand.Count;
            var competencesWithGaps = rows.Count(r => r.Status == "GAP");
            var criticalCompetences = rows.Count(r => r.EmployeesAvailable <= 1 && r.ProjectsRequiring > 0);

            var vm = new HRSkillGapOverviewViewModel
            {
                TotalProjectRequirements = totalProjectRequirements,
                UniqueRequiredCompetences = uniqueRequiredCompetences,
                CompetencesWithGaps = competencesWithGaps,
                CriticalCompetences = criticalCompetences,
                Rows = rows
                    .OrderByDescending(r => r.Status == "GAP")
                    .ThenByDescending(r => r.ProjectsRequiring)
                    .ThenBy(r => r.CompetenceName)
                    .ToList()
            };

            return View(vm);
        }

        // ==================================
        // HR Skill Gap - Details per skill
        // ==================================
        [HttpGet]
        public async Task<IActionResult> SkillGapDetails(int competenceId)
        {
            var competence = await _context.Competences.FirstOrDefaultAsync(c => c.Id == competenceId);
            if (competence == null)
            {
                return RedirectToAction(nameof(SkillGap));
            }

            // Demand: which projects require this competence?
            var projectDemand = await _context.ProjectRequirements
                .Include(r => r.Project)
                .Where(r => r.CompetenceId == competenceId)
                .GroupBy(r => new { r.ProjectId, ProjectName = r.Project.Name })
                .Select(g => new HRSkillGapProjectDemandRow
                {
                    ProjectId = g.Key.ProjectId,
                    ProjectName = g.Key.ProjectName ?? $"ProjectId {g.Key.ProjectId}",
                    MinLevel = g.Max(x => x.MinLevel),
                    MinYears = g.Max(x => x.MinYearsOfExperience)
                })
                .OrderBy(x => x.ProjectName)
                .ToListAsync();

            // Supply: which employees have it, and at what level?
            var employeeSupply = await _context.UserCompetences
                .Include(uc => uc.User)
                .Where(uc => uc.CompetenceId == competenceId)
                .OrderByDescending(uc => uc.Level)
                .ThenByDescending(uc => uc.YearsOfExperience)
                .ThenBy(uc => uc.User.Email)
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

            var projectsRequiring = projectDemand.Count;
            var employeesAvailable = employeeSupply.Count;

            var summary = $"'{competence.Name}' is required in {projectsRequiring} project(s). " +
                          $"Currently, {employeesAvailable} employee(s) have this competence.";

            if (employeesAvailable < projectsRequiring)
                summary += " There is a GAP risk. Consider training or recruitment.";
            else if (employeesAvailable == projectsRequiring)
                summary += " This is TIGHT (just enough). Consider backup resources.";
            else
                summary += " Supply looks OK.";

            var vm = new HRSkillGapDetailsViewModel
            {
                CompetenceId = competence.Id,
                CompetenceName = competence.Name ?? "",
                ProjectDemand = projectDemand,
                EmployeeSupply = employeeSupply,
                ProjectsRequiring = projectsRequiring,
                EmployeesAvailable = employeesAvailable,
                SummaryMessage = summary
            };

            return View(vm);
        }


    }
}
