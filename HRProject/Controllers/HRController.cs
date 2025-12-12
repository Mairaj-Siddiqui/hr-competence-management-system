using System.Linq;                                   // Gives us LINQ methods like Count(), GroupBy(), Select()
using System.Threading.Tasks;                       // Needed for async/await and Task<IActionResult>
using HRProject.Data;                               // To access ApplicationDbContext (your EF Core DbContext)
using HRProject.Models;                             // To use ApplicationUser and HRDashboardViewModel
using Microsoft.AspNetCore.Authorization;           // For the [Authorize] attribute
using Microsoft.AspNetCore.Identity;                // For UserManager<ApplicationUser>
using Microsoft.AspNetCore.Mvc;                     // For Controller and IActionResult

namespace HRProject.Controllers                      // Same namespace pattern as other controllers
{
    /// <summary>
    /// This controller contains actions that are intended for the HR role.
    /// For now, we only implement an HR Dashboard.
    /// </summary>
    ///[Authorize(Roles = "Admin,HR")]                 // Only users in Admin OR HR roles can access this controller
    public class HRController : Controller
    {
        // Field for the database context, used to query Competences, Users, UserCompetences, etc.
        private readonly ApplicationDbContext _context;

        // Field for UserManager, used if we want to query ApplicationUser objects
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Constructor: ASP.NET Core will inject ApplicationDbContext and UserManager<ApplicationUser>.
        /// This is called "dependency injection".
        /// </summary>
        public HRController(
            ApplicationDbContext context,                 // The EF Core database context
            UserManager<ApplicationUser> userManager      // The ASP.NET Identity user manager
        )
        {
            _context = context;                           // Save the context into the private field
            _userManager = userManager;                   // Save the user manager into the private field
        }

        /// <summary>
        /// This action shows the main HR Dashboard.
        /// It gathers simple statistics from the database and passes them to the view.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // ------------------------------
            // 1. Get all users from the system
            // ------------------------------

            // We use UserManager.Users to get an IQueryable<ApplicationUser>.
            // Then we count how many users there are in total.
            var totalEmployees = _userManager.Users.Count();

            // ------------------------------
            // 2. Get total number of competences in the catalogue
            // ------------------------------

            // We use the Competences DbSet on our ApplicationDbContext.
            var totalCompetences = _context.Competences.Count();

            // ------------------------------
            // 3. Get how many distinct users have competences
            // ------------------------------

            // _context.UserCompetences contains one row per (User, Competence) pair.
            // We select only the UserId field, and call Distinct() to avoid duplicates,
            // then Count() to get the number of UNIQUE users that have at least one competence.
            var usersWithCompetences = _context.UserCompetences
                .Select(uc => uc.UserId)
                .Distinct()
                .Count();

            // Users without competences = all users - users that have at least one competence.
            var usersWithoutCompetences = totalEmployees - usersWithCompetences;

            // ------------------------------
            // 4. Top competences (most common competences)
            // ------------------------------

            // We group UserCompetences by competence name,
            // then count how many different users have that competence,
            // sort descending by user count,
            // and take the top 5 to keep it simple.
            var topCompetences = _context.UserCompetences
                .GroupBy(uc => uc.Competence.Name)                 // Group rows by competence name
                .Select(group => new HRDashboardCompetenceSummary  // Create a summary object for each group
                {
                    CompetenceName = group.Key,                    // The name of the competence
                    UserCount = group
                        .Select(uc => uc.UserId)                   // Take user ids
                        .Distinct()                                // Make them unique
                        .Count()                                   // Count how many unique users have this skill
                })
                .OrderByDescending(x => x.UserCount)               // Sort by popularity (most used first)
                .Take(5)                                           // Take only the top 5 competences
                .ToList();                                         // Execute the query and get a List<HRDashboardCompetenceSummary>

            // ------------------------------
            // 5. Create the ViewModel and fill it with all calculated values
            // ------------------------------

            var viewModel = new HRDashboardViewModel
            {
                TotalEmployees = totalEmployees,                   // Total number of users in the system
                TotalCompetences = totalCompetences,               // Total number of competences in the catalogue
                UsersWithCompetences = usersWithCompetences,       // Users that have at least one competence
                UsersWithoutCompetences = usersWithoutCompetences, // Users that have no competence yet
                TopCompetences = topCompetences                    // List of top 5 competences
            };

            // ------------------------------
            // 6. Pass the ViewModel to the view
            // ------------------------------

            // We return the "Index" view inside Views/HR/Index.cshtml,
            // and pass our viewModel object as the model to that view.
            return View(viewModel);
        }
    }
}
