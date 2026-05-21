using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.AspNetCore.Authorization;
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

        public async Task<IActionResult> List(int? categoryId, string? search, int page = 1, int pageSize = 10)
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

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Title.Contains(search) || c.ShortDescription.Contains(search));
                ViewBag.SearchQuery = search;
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
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            ViewBag.InstructorCourseCount = await _context.Courses
                .CountAsync(c => c.InstructorId == course.InstructorId);

            if (User.Identity.IsAuthenticated && User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                var existingReview = course.Reviews.FirstOrDefault(r => r.StudentId == user.Id);
                ViewBag.UserRating = existingReview?.Rating ?? 0;
            }

            return View(course);
        }

        // GET: /Course/Search?query=react
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Search(string query, int? categoryId)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return Json(new List<object>());

            var queryable = _context.Courses
                .Include(c => c.Instructor)
                .Where(c => c.Title.Contains(query) || c.ShortDescription.Contains(query))
                .AsQueryable();

            // Фильтр по категории
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                queryable = queryable.Where(c => c.CategoryId == categoryId.Value);
            }

            var courses = await queryable
                .OrderBy(c => c.Title)
                .Take(8)
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    Instructor = c.Instructor.FirstName + " " + c.Instructor.LastName,
                    c.Price,
                    ImagePath = string.IsNullOrEmpty(c.ImagePath) || !c.ImagePath.StartsWith("/") ? "/img/default.jpg" : c.ImagePath
                })
                .ToListAsync();

            return Json(courses);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RateCourse(int courseId, int rating)
        {
            if (rating < 1 || rating > 5)
                return Json(new { success = false, message = "Invalid rating" });

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Not authorized" });

            if (!User.IsInRole("Student") || User.IsInRole("Instructor"))
                return Json(new { success = false, message = "Only enrolled students can rate" });

            var enrollment = await _context.Enrollments
                .AnyAsync(e => e.StudentId == user.Id && e.CourseId == courseId);
            if (!enrollment)
                return Json(new { success = false, message = "You must purchase the course first" });

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.StudentId == user.Id && r.CourseId == courseId);

            if (review == null)
            {
                review = new Review
                {
                    StudentId = user.Id,
                    CourseId = courseId,
                    Rating = rating,
                    CreatedAt = DateTime.Now
                };
                _context.Reviews.Add(review);
            }
            else
            {
                review.Rating = rating;
                review.CreatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            var avg = await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;
            var count = await _context.Reviews.CountAsync(r => r.CourseId == courseId);

            return Json(new { success = true, average = avg.ToString("0.0"), count = count });
        }
    }
}
