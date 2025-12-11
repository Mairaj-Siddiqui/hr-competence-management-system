using HRProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRProject.Controllers
{
    //Authorize(Roles = "Admin,HR")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;  // 

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)  // 
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Create role page (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create role (POST)
        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                // Check if role already exists
                bool exists = await _roleManager.RoleExistsAsync(roleName);

                if (!exists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    ViewBag.Message = "Role created successfully";
                }
                else
                {
                    ViewBag.Message = "Role already exists";
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Assign()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Assign(string email, string roleName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(roleName))
            {
                ViewBag.Message = "Email and Role are required.";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Message = "User not found.";
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                ViewBag.Message = "Role does not exist.";
                return View();
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
                ViewBag.Message = $"User '{email}' added to role '{roleName}'.";
            }
            else
            {
                ViewBag.Message = $"User '{email}' is already in role '{roleName}'.";
            }

            return View();
        }
    }
}
    

