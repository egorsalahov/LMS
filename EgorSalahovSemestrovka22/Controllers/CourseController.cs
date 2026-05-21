using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Student> _userManager;

        public CourseController(AppDbContext context, UserManager<Student> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                .Select(cat => new { cat.Id, cat.Name, Count = cat.Courses.Count })
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.PageSize = pageSize;

            // ID курсов в Wishlist текущего пользователя
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    ViewBag.WishlistCourseIds = await _context.Wishlists
                        .Where(w => w.StudentId == user.Id)
                        .Select(w => w.CourseId)
                        .ToListAsync();
                }
            }

            return View(courses);
        }

        // GET: /Course/Category – список всех категорий
        public async Task<IActionResult> Category()
        {
            var categories = await _context.Categories
                .Include(c => c.Courses)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        // GET: /Course/CategoryCourses/5 – курсы конкретной категории
        public async Task<IActionResult> CategoryCourses(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Courses)
                    .ThenInclude(c => c.Instructor)
                .Include(c => c.Courses)
                    .ThenInclude(c => c.Reviews)
                .Include(c => c.Courses)
                    .ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .Include(c => c.Reviews)
                .Include(c => c.Enrollments)   // <-- Добавить
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            ViewBag.InstructorCourseCount = await _context.Courses
                .CountAsync(c => c.InstructorId == course.InstructorId);

            return View(course);
        }
    }
}
