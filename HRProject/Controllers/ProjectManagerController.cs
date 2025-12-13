using Microsoft.AspNetCore.Mvc;

namespace HRProject.Controllers
{
    public class ProjectManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Find()
        {
            return View();
        }
    }
}
