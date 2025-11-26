using System.Linq;
using System.Threading.Tasks;
using HRProject.Data;
using HRProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    // Remove [Authorize] for now to avoid any hidden role issues
    public class CompetencesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompetencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Competences
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var competences = await _context.Competences
                .OrderBy(c => c.Id)
                .ToListAsync();

            ViewBag.Database = _context.Database.GetDbConnection().Database;
            ViewBag.Count = competences.Count;

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
        public async Task<IActionResult> Create(string name, string description)
        {
            // Very, very simple – no model binding magic
            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.Error = "Name is required.";
                return View();
            }

            var competence = new Competence
            {
                Name = name,
                Description = description
            };

            _context.Competences.Add(competence);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Created competence '{competence.Name}' with Id {competence.Id}";

            return RedirectToAction(nameof(Index));
        }

        
    }
}
