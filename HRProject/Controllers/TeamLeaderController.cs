using HRProject.Data;
using HRProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRProject.Controllers
{
    [Authorize]
    
    public class TeamLeaderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeamLeaderController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ============================
        // LIST TEAMS
        // ============================
        public async Task<IActionResult> Index()
        {
            var leaderId = _userManager.GetUserId(User);

            var teams = await _db.TeamLeaders
                .Where(t => t.LeaderUserId == leaderId)
                .ToListAsync();

            return View(teams);
        }

        // ============================
        // CREATE TEAM (GET)
        // ============================
        public IActionResult Create()
        {
            return View();
        }

        // ============================
        // CREATE TEAM (POST)
        // ============================
        [HttpPost]
        public async Task<IActionResult> Create(TeamLeader model)
        {
            // Minst en capacity måste vara ifylld
            if (model.RequiredHours == null &&
                model.RequiredDays == null &&
                model.RequiredMonths == null)
            {
                ModelState.AddModelError("", "You must enter at least one capacity value (hours, days or months).");
                return View(model);
            }

            var leaderId = _userManager.GetUserId(User);
            model.LeaderUserId = leaderId;

            _db.TeamLeaders.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ============================
        // DASHBOARD
        // ============================
        public async Task<IActionResult> Dashboard(int id)
        {
            var leaderId = _userManager.GetUserId(User);

            var team = await _db.TeamLeaders
                .Include(t => t.Members).ThenInclude(m => m.User)
                .Include(t => t.SkillNeeds).ThenInclude(s => s.Competence)
                .Include(t => t.GrowthPlans)
                .FirstOrDefaultAsync(t => t.Id == id && t.LeaderUserId == leaderId);

            if (team == null)
                return NotFound();

            return View(team);
        }

        // ============================
        // EDIT TEAM (GET)
        // ============================
        public async Task<IActionResult> Edit(int id)
        {
            var leaderId = _userManager.GetUserId(User);

            var team = await _db.TeamLeaders
                .FirstOrDefaultAsync(t => t.Id == id && t.LeaderUserId == leaderId);

            if (team == null) return NotFound();

            return View(team);
        }

        // ============================
        // EDIT TEAM (POST)
        // ============================
        [HttpPost]
        public async Task<IActionResult> Edit(TeamLeader model)
        {
            var leaderId = _userManager.GetUserId(User);

            var team = await _db.TeamLeaders
                .FirstOrDefaultAsync(t => t.Id == model.Id && t.LeaderUserId == leaderId);

            if (team == null) return NotFound();

            // Minst ETT capacity-fält måste vara ifyllt
            if (model.RequiredHours == null &&
                model.RequiredDays == null &&
                model.RequiredMonths == null)
            {
                ModelState.AddModelError("", "You must enter at least one capacity value (hours, days or months).");
                return View(model);
            }

            // Update fields
            team.TeamName = model.TeamName;
            team.Description = model.Description;
            team.RequiredHours = model.RequiredHours;
            team.RequiredDays = model.RequiredDays;
            team.RequiredMonths = model.RequiredMonths;

            await _db.SaveChangesAsync();

            return RedirectToAction("Dashboard", new { id = team.Id });
        }

        // ============================
        // ADD MEMBER
        // ============================
        public async Task<IActionResult> AddMember(int id)
        {
            var team = await _db.TeamLeaders.FindAsync(id);
            if (team == null) return NotFound();

            ViewBag.TeamId = id;
            return View(await _db.Users.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int teamId, string userId)
        {
            bool exists = await _db.TeamMembers
                .AnyAsync(m => m.TeamLeaderId == teamId && m.UserId == userId);

            if (exists)
            {
                TempData["Error"] = "This user is already added to your team.";
                return RedirectToAction("Dashboard", new { id = teamId });
            }

            _db.TeamMembers.Add(new TeamMember
            {
                TeamLeaderId = teamId,
                UserId = userId
            });

            await _db.SaveChangesAsync();
            return RedirectToAction("Dashboard", new { id = teamId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMember(int id, int teamId)
        {
            var member = await _db.TeamMembers.FindAsync(id);
            if (member == null) return NotFound();

            _db.TeamMembers.Remove(member);
            await _db.SaveChangesAsync();

            return RedirectToAction("Dashboard", new { id = teamId });
        }

        // ============================
        // ADD SKILL NEED
        // ============================
        public async Task<IActionResult> AddSkillNeed(int id)
        {
            if (await _db.TeamLeaders.FindAsync(id) == null) return NotFound();

            ViewBag.TeamId = id;
            ViewBag.Competences = await _db.Competences.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSkillNeed(int teamId, int competenceId, int levelNeeded, string importance)
        {
            bool exists = await _db.TeamSkillNeeds.AnyAsync(s =>
                s.TeamLeaderId == teamId &&
                s.CompetenceId == competenceId &&
                s.LevelNeeded == levelNeeded &&
                s.Importance == importance
            );

            if (exists)
            {
                TempData["Error"] = "This skill with the same level and importance is already added.";
                return RedirectToAction("Dashboard", new { id = teamId });
            }

            _db.TeamSkillNeeds.Add(new TeamSkillNeed
            {
                TeamLeaderId = teamId,
                CompetenceId = competenceId,
                LevelNeeded = levelNeeded,
                Importance = importance
            });

            await _db.SaveChangesAsync();
            return RedirectToAction("Dashboard", new { id = teamId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSkillNeed(int id, int teamId)
        {
            var skill = await _db.TeamSkillNeeds.FindAsync(id);
            if (skill == null) return NotFound();

            _db.TeamSkillNeeds.Remove(skill);
            await _db.SaveChangesAsync();

            return RedirectToAction("Dashboard", new { id = teamId });
        }

        // ============================
        // ADD GROWTH PLAN
        // ============================
        public async Task<IActionResult> AddGrowthPlan(int id)
        {
            if (await _db.TeamLeaders.FindAsync(id) == null) return NotFound();

            ViewBag.TeamId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddGrowthPlan(int teamId, TeamGrowthPlan model)
        {
            model.TeamLeaderId = teamId;

            _db.TeamGrowthPlans.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Dashboard", new { id = teamId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGrowthPlan(int id, int teamId)
        {
            var plan = await _db.TeamGrowthPlans.FindAsync(id);
            if (plan == null) return NotFound();

            _db.TeamGrowthPlans.Remove(plan);
            await _db.SaveChangesAsync();

            return RedirectToAction("Dashboard", new { id = teamId });
        }

        // ============================
        // DELETE ENTIRE TEAM
        // ============================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var team = await _db.TeamLeaders
                .Include(t => t.Members)
                .Include(t => t.SkillNeeds)
                .Include(t => t.GrowthPlans)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null) return NotFound();

            _db.TeamMembers.RemoveRange(team.Members);
            _db.TeamSkillNeeds.RemoveRange(team.SkillNeeds);
            _db.TeamGrowthPlans.RemoveRange(team.GrowthPlans);
            _db.TeamLeaders.Remove(team);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
