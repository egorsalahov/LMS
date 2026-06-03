using EgorSalahovSemestrovka22.Models;
using Microsoft.AspNetCore.Mvc;
using Sem.Web.Services;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeService _homeService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(HomeService homeService, ILogger<HomeController> logger)
        {
            _homeService = homeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _homeService.GetCategoriesWithCoursesAsync();
            ViewBag.PopularCourses = await _homeService.GetPopularCoursesAsync();
            ViewBag.InstructorCount = await _homeService.GetInstructorCountAsync();
            ViewBag.StudentCount = await _homeService.GetStudentCountAsync();
            ViewBag.CourseCount = await _homeService.GetCourseCountAsync();

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