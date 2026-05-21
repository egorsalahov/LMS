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

        // GET: /Admin/ECommerce/Customers
        public async Task<IActionResult> Customers()
        {
            var instructors = await _context.Instructors
                .Include(i => i.Courses)
                .OrderBy(i => i.FirstName)
                .Select(i => new
                {
                    i.Id,
                    FullName = i.FirstName + " " + i.LastName,
                    i.Email,
                    CoursesCount = i.Courses.Count,
                    TotalEarnings = i.TotalEarnings,
                    i.RegistrationDate
                })
                .ToListAsync();

            return View(instructors);
        }

        // GET: /Admin/ECommerce/Orders
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.Student)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    CustomerName = o.Student != null ? o.Student.FirstName + " " + o.Student.LastName : "Unknown",
                    o.OrderDate,
                    o.OrderStatus
                })
                .ToListAsync();

            return View(orders);
        }
    }
}