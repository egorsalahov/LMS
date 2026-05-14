using EgorSalahovSemestrovka22.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DataAdminController : Controller
    {
        private readonly AppDbContext _context;

        public DataAdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CoursesData()
        {
            var courses = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .OrderBy(c => c.Id)
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> StudentsData()
        {
            var students = await _context.Users
                .Include(s => s.Enrollments)
                .Include(s => s.Orders)
                .OrderBy(s => s.Id)
                .ToListAsync();

            return View(students);
        }
    }
}
