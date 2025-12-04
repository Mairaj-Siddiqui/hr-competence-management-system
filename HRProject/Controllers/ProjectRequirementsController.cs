using System.Threading.Tasks;
using HRProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    [Authorize(Roles = "Admin,HR,ProjectManager,ProjectLeader")]
    public class ProjectRequirementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectRequirementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ProjectRequirements
        public async Task<IActionResult> Index()
        {
            var requirements = await _context.ProjectRequirements
                .Include(r => r.Project)
                .Include(r => r.Competence)
                .ToListAsync();

            return View(requirements);   // Views/ProjectRequirements/Index.cshtml
        }
    }
}
