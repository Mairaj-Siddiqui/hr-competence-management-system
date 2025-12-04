using System.Threading.Tasks;
using HRProject.Data;
using HRProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    [Authorize(Roles = "Admin,HR,ProjectLeader")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MatchService _matchService;

        public ProjectsController(ApplicationDbContext context, MatchService matchService)
        {
            _context = context;
            _matchService = matchService;
        }

        // List projects (very simple)
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects.ToListAsync();
            return View(projects);
        }

        // Show top matches for a given project
        public async Task<IActionResult> Matches(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Requirements)
                .ThenInclude(r => r.Competence)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            var matches = await _matchService.GetTopMatchesForProjectAsync(id, topN: 10);

            ViewBag.ProjectName = project.Name;

            return View(matches);   // Views/Projects/Matches.cshtml
        }
    }
}
