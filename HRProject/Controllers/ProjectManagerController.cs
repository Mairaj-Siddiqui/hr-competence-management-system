using HRProject.Data;
using HRProject.Models;
using Microsoft.AspNetCore.Mvc;

public class ProjectManagerController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectManagerController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
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


    public IActionResult Find()
    {
        return View();
    }
}
