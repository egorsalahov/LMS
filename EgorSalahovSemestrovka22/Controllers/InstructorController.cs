using Microsoft.AspNetCore.Mvc;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class InstructorController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult MyProfile()
        {
            return View();
        }
        public IActionResult Courses()
        {
            return View();
        }
        public IActionResult Students()
        {
            return View();
        }
        public IActionResult Settings()
        {
            return View();
        }
    }
}
