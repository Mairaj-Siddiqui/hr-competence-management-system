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
    }
}
