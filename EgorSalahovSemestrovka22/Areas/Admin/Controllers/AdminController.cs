using Microsoft.AspNetCore.Mvc;

namespace EgorSalahovSemestrovka22.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ECommerce()
        {
            return View();
        }
    }
}
