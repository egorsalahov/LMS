using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Services;
using System.Diagnostics;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeService _homeService;

        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
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