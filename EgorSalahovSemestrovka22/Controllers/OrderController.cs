using Microsoft.AspNetCore.Mvc;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
