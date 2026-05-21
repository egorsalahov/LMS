using System.Diagnostics;
using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Категории
            var categories = await _context.Categories
                .Include(c => c.Courses)
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.Categories = categories;

            // Популярные курсы (по количеству записавшихся студентов)
            var popularCourses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(6)
                .ToListAsync();
            ViewBag.PopularCourses = popularCourses;

            // Количества
            ViewBag.InstructorCount = await _context.Instructors.CountAsync();
            ViewBag.StudentCount = await _context.Users.CountAsync();
            ViewBag.CourseCount = await _context.Courses.CountAsync();

            return View();
        }   
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode == 404) return View("NotFound");
            if (statusCode == 403) return View("AccessDenied");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}