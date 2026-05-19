using EgorSalahovSemestrovka22.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> List(int? categoryId, int page = 1, int pageSize = 10)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
                ViewBag.SelectedCategory = categoryId.Value;
            }

            var totalCourses = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCourses / (double)pageSize);

            var courses = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories
                .Select(cat => new
                {
                    cat.Id,
                    cat.Name,
                    Count = cat.Courses.Count
                })
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.PageSize = pageSize;

            return View(courses);
        }

        public IActionResult Category()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .Include(c => c.Reviews)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            // Количество курсов этого инструктора
            ViewBag.InstructorCourseCount = await _context.Courses
                .CountAsync(c => c.InstructorId == course.InstructorId);

            return View(course);
        }
    }
}
