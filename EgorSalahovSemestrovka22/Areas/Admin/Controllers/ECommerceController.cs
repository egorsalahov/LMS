using EgorSalahovSemestrovka22.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ECommerceController : Controller
    {

        private readonly AppDbContext _context;

        public ECommerceController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Products()
        {
            var courses = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .OrderBy(c => c.Id)
                .ToListAsync();

            return View(courses);
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
