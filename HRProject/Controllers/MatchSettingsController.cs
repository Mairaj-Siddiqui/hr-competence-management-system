using System.Threading.Tasks;
using HRProject.Data;
using HRProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    [Authorize(Roles = "Admin")]   // only Admin can change weights
    public class MatchSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatchSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /MatchSettings/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // We only ever want ONE row in this table
            var settings = await _context.MatchSettings.FirstOrDefaultAsync();

            if (settings == null)
            {
                settings = new MatchSettings();  // uses default values (50/30/20)
                _context.MatchSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            return View(settings);  // Views/MatchSettings/Edit.cshtml
        }

        // POST: /MatchSettings/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MatchSettings model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var settings = await _context.MatchSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new MatchSettings();
                _context.MatchSettings.Add(settings);
            }

            settings.CompetenceWeight = model.CompetenceWeight;
            settings.ExperienceWeight = model.ExperienceWeight;
            settings.AvailabilityWeight = model.AvailabilityWeight;

            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Match settings saved.";
            return View(settings);
        }
    }
}
