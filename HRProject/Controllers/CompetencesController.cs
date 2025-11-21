using System.Linq;
using System.Threading.Tasks;
using HRProject.Data;
using HRProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    // Only Admin and HR can manage competences
    [Authorize(Roles = "Admin,HR")]
    public class CompetencesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompetencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Competences
        public async Task<IActionResult> Index()
        {
            var competences = await _context.Competences
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(competences);
        }

        // GET: /Competences/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Competences/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Competence model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Optional: avoid duplicates
            bool exists = await _context.Competences
                .AnyAsync(c => c.Name == model.Name);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "A competence with this name already exists.");
                return View(model);
            }

            _context.Competences.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
