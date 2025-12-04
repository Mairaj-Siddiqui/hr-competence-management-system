using HRProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    public class ProjectController : Controller
    {
        [Authorize(Roles = "Admin,HR,ProjectManager,ProjectLeader")]
        public class ProjectsController : Controller
        {
            private readonly ApplicationDbContext _context;

            public ProjectsController(ApplicationDbContext context)
            {
                _context = context;
            }

            // GET: /Projects
            public async Task<IActionResult> Index()
            {
                var projects = await _context.Projects.ToListAsync();
                return View(projects);   // Views/Projects/Index.cshtml
            }
        }
    }
}
