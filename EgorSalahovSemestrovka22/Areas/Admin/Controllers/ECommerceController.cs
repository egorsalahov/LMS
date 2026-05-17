using Microsoft.AspNetCore.Mvc;

namespace EgorSalahovSemestrovka22.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ECommerceController : Controller
    {
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult Customers()
        {
            return View();
        }
        public IActionResult Orders()
        {
            return View();
        }
    }
}
