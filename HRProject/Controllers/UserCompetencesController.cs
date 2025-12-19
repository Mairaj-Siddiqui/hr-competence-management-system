using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRProject.Data;
using HRProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    /// [Authorize(Roles = "Admin,HR")]  // you can re-enable later if you like
    public class UserCompetencesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserCompetencesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /UserCompetences/Assign
        [HttpGet]
        public async Task<IActionResult> Assign()
        {
            var model = new AssignCompetenceViewModel();
            await FillDropdownsAsync(model);
            // No user selected yet, so table stays empty
            return View("~/Views/Competences/Assign.cshtml", model);
        }

        // POST: /UserCompetences/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignCompetenceViewModel model)
        {
            await FillDropdownsAsync(model);

            // --- manual required checks (for clear messages) ---
            if (string.IsNullOrWhiteSpace(model.UserEmail))
            {
                ModelState.AddModelError(string.Empty, "User email is required.");
                return View("~/Views/Competences/Assign.cshtml", model);
            }

            if (model.CompetenceId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a competence.");
                return View("~/Views/Competences/Assign.cshtml", model);
            }

            if (model.Level <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a level.");
                return View("~/Views/Competences/Assign.cshtml", model);
            }

            // --- find user ---
            var user = await _userManager.FindByEmailAsync(model.UserEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found. Email must match exactly.");
                return View("~/Views/Competences/Assign.cshtml", model);
            }

            // --- find competence ---
            var competence = await _context.Competences
                .FirstOrDefaultAsync(c => c.Id == model.CompetenceId);

            if (competence == null)
            {
                ModelState.AddModelError(string.Empty, "Competence not found.");
                return View("~/Views/Competences/Assign.cshtml", model);
            }

            // --- check if this user already has this competence ---
            var existing = await _context.UserCompetences
                .FirstOrDefaultAsync(uc => uc.UserId == user.Id &&
                                           uc.CompetenceId == competence.Id);

            if (existing == null)
            {
                // create new row
                var userCompetence = new UserCompetence
                {
                    UserId = user.Id,
                    CompetenceId = competence.Id,
                    Level = (CompetenceLevel)model.Level,
                    YearsOfExperience = model.YearsOfExperience
                };

                _context.UserCompetences.Add(userCompetence);
                ViewBag.SuccessMessage = $"Competence '{competence.Name}' was assigned to {user.Email}.";
            }
            else
            {
                // update existing row
                existing.Level = (CompetenceLevel)model.Level;
                existing.YearsOfExperience = model.YearsOfExperience;

                ViewBag.SuccessMessage = $"Competence '{competence.Name}' for {user.Email} was updated.";
            }

            await _context.SaveChangesAsync();

            // Load all competences for this user for the table
            model.ExistingUserCompetences = await _context.UserCompetences
                .Where(uc => uc.UserId == user.Id)
                .Include(uc => uc.Competence)
                .ToListAsync();

            return View("~/Views/Competences/Assign.cshtml", model);
        }


        private async Task FillDropdownsAsync(AssignCompetenceViewModel model)
        {
            model.Competences = await _context.Competences
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            model.Levels = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Basic" },
                new SelectListItem { Value = "2", Text = "Intermediate" },
                new SelectListItem { Value = "3", Text = "Advanced" }
            };
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string email)
        {
            // If no email is passed, use the currently logged-in user
            ApplicationUser user;

            if (string.IsNullOrWhiteSpace(email))
            {
                user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    // not logged in
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                email = user.Email;
            }
            else
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Email == email);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View("~/Views/Competences/Profile.cshtml", null);
            }

            var userCompetences = await _context.UserCompetences
                .Where(uc => uc.UserId == user.Id)
                .Include(uc => uc.Competence)
                .ToListAsync();

            var model = new UserCompetenceProfileViewModel
            {
                UserEmail = user.Email,
                FullName = user.FullName,
                JobTitle = user.JobTitle,
                Competences = userCompetences
            };

            return View("~/Views/Competences/Profile.cshtml", model);
        }

        // get all competence for all users - for admin/HR overview
        //[HttpGet]
        //public async Task<IActionResult> AllUserCompetences()
        //{
        //    var allUserCompetences = await _context.UserCompetences
        //        .Include(uc => uc.User)
        //        .Include(uc => uc.Competence)
        //        .OrderBy(uc => uc.User.Email)
        //        .ThenBy(uc => uc.Competence.Name)
        //        .ToListAsync();
        //    return View("~/Views/Competences/AllUserCompetences.cshtml", allUserCompetences);

        //}

        public async Task<IActionResult> AllUserCompetences()
        {
            var users = await _userManager.Users
                .Include(u => u.UserCompetences)
                    .ThenInclude(uc => uc.Competence)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return View("~/Views/Competences/AllUserCompetences.cshtml", users);
        }




    }
    

}