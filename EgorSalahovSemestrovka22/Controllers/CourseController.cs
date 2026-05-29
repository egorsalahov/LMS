using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Services;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;
        private readonly UserManager<Student> _userManager;

        public CourseController(CourseService courseService, UserManager<Student> userManager)
        {
            _courseService = courseService;
            _userManager = userManager;
        }

        public async Task<IActionResult> List(int? categoryId, string? search, int page = 1, int pageSize = 10,
            string? priceType = null, Level? level = null, decimal? priceFrom = null, decimal? priceTo = null)
        {
            var (courses, totalCount) = await _courseService.GetFilteredCoursesAsync(
                categoryId, search, level, priceType, priceFrom, priceTo, page, pageSize);

            ViewBag.Categories = await _courseService.GetCategoryListAsync();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SelectedLevel = level;
            ViewBag.SelectedPriceType = priceType;
            ViewBag.PriceFrom = priceFrom;
            ViewBag.PriceTo = priceTo;
            ViewBag.SearchQuery = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCourses = totalCount;
            ViewBag.PageSize = pageSize;

            if (User.Identity.IsAuthenticated)
                ViewBag.WishlistCourseIds = await _courseService.GetWishlistCourseIdsAsync(_userManager.GetUserId(User));

            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> ListAjax(int? categoryId, string? search, int page = 1, int pageSize = 10,
            string? priceType = null, Level? level = null, decimal? priceFrom = null, decimal? priceTo = null)
        {
            var result = await _courseService.GetFilteredCoursesAjaxAsync(
                categoryId, search, level, priceType, priceFrom, priceTo, page, pageSize);
            return Json(result);
        }

        public async Task<IActionResult> Category()
        {
            var categories = await _courseService.GetAllCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> CategoryCourses(int id)
        {
            var category = await _courseService.GetCategoryWithCoursesAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var course = await _courseService.GetCourseDetailAsync(id);
            if (course == null) return NotFound();

            ViewBag.InstructorCourseCount = await _courseService.GetInstructorCourseCountAsync(course.InstructorId);

            if (User.Identity.IsAuthenticated && User.IsInRole("Student"))
                ViewBag.UserRating = course.Reviews.FirstOrDefault(r => r.StudentId == _userManager.GetUserId(User))?.Rating ?? 0;

            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query, int? categoryId)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return Json(new List<object>());

            var results = await _courseService.SearchCoursesAsync(query, categoryId);
            return Json(results);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> RateCourse(int courseId, int rating)
        {
            var userId = _userManager.GetUserId(User);
            var isStudent = User.IsInRole("Student") && !User.IsInRole("Instructor");
            var (success, message, avg, count) = await _courseService.RateCourseAsync(userId, courseId, rating, isStudent);

            if (!success) return Json(new { success = false, message });
            return Json(new { success = true, average = avg.ToString("0.0"), count });
        }
    }
}
