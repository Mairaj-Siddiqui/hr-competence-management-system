using System.Threading.Tasks;
using HRProject.Data;
using HRProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalCompetences = await _context.Competences.CountAsync(),
                TotalUserCompetences = await _context.UserCompetences.CountAsync(),
                //TotalProjects = await _context.Projects.CountAsync(),
                //TotalProjectRequirements = await _context.ProjectRequirements.CountAsync()
            };

            return View(model); // Views/Admin/Index.cshtml
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {

            {
                var users = await _userManager.Users.ToListAsync();
                var model = new List<UserWithRolesViewModel>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    model.Add(new UserWithRolesViewModel
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Roles = roles.ToList(),
                        IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow
                    });
                }

                // list of all role names for dropdown
                ViewBag.AllRoles = await _roleManager.Roles
                    .Select(r => r.Name)
                    .ToListAsync();

                return View(model);
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(string userId, string newRole)
        {
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction(nameof(Users));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction(nameof(Users));

            var currentRoles = await _userManager.GetRolesAsync(user);

            // remove all current roles
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // add new role if selected
            if (!string.IsNullOrEmpty(newRole))
            {
                await _userManager.AddToRoleAsync(user, newRole);
            }

            TempData["UsersMessage"] = "User role updated.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserActive(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction(nameof(Users));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction(nameof(Users));

            // prevent deactivating yourself (optional safety)
            if (user.Email == User?.Identity?.Name)
            {
                TempData["UsersMessage"] = "You cannot deactivate your own account.";
                return RedirectToAction(nameof(Users));
            }

            var now = DateTimeOffset.UtcNow;

            if (user.LockoutEnd.HasValue && user.LockoutEnd > now)
            {
                // currently locked -> activate
                user.LockoutEnd = null;
                user.LockoutEnabled = false;
                TempData["UsersMessage"] = "User activated.";
            }
            else
            {
                // currently active -> deactivate
                user.LockoutEnabled = true;
                user.LockoutEnd = now.AddYears(100); // "forever"
                TempData["UsersMessage"] = "User deactivated.";
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Users));
        }


    }
}












