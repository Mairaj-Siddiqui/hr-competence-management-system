using HRProject.Data;
using HRProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProjectManagerController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectManagerController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _context.ProjectManager.ToListAsync();
        return View(projects);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectManager project)
    {
        if (!ModelState.IsValid)
            return View(project);

        _context.ProjectManager.Add(project);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> AssignUser(int projectId)
    {
        var project = await _context.ProjectManager.FindAsync(projectId);
        if (project == null) return NotFound();

        ViewBag.Users = await _context.Users.ToListAsync();
        ViewBag.ProjectId = projectId;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignUser(int projectId, string userId, string role)
    {
        var assignment = new ProjectTeamMember
        {
            ProjectId = projectId,
            UserId = userId,
            AssignedRole = role
        };

        _context.ProjectTeamMembers.Add(assignment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var project = await _context.ProjectManager
            .Include(p => p.TeamMembers)
            .ThenInclude(tm => tm.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }


    public IActionResult Find()
    {
        return View();
    }
}
