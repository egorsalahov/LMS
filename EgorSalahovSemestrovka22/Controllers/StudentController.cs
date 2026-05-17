using Microsoft.AspNetCore.Mvc;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult MyProfile()
        {
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }
        public IActionResult Wishlist()
        {
            return View();
        }
        public IActionResult OrderHistory()
        {
            return View();
        }
        public IActionResult Settings()
        {
            return View();
        }
        public IActionResult BecomeInstructor()
        {
            return View();
        }
    }
}
