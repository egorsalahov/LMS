using EgorSalahovSemestrovka22.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DataAdminController : Controller
    {
        private readonly AppDbContext _context;

        public DataAdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> InstructorsData()
        {
            var instructors = await _context.Instructors
                .Include(i => i.Courses)
                .Include(i => i.Educations)
                .Include(i => i.Experiences)
                .OrderBy(i => i.Id)
                .ToListAsync();

            return View(instructors);
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
