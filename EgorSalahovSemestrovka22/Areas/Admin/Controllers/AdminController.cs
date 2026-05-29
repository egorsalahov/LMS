using EgorSalahovSemestrovka22.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Areas.Admin.Services;

namespace EgorSalahovSemestrovka22.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _adminService.GetDashboardDataAsync();
            ViewBag.TotalUsers = data.TotalUsers;
            ViewBag.NewUsers = data.NewUsers;
            ViewBag.RecentOrders = data.RecentOrders;
            ViewBag.RegistrationData = data.RegistrationChartData;
            return View();
        }

        public async Task<IActionResult> ECommerce()
        {
            var data = await _adminService.GetECommerceDataAsync();
            ViewBag.TotalOrders = data.TotalOrders;
            ViewBag.TotalSales = data.TotalSales;
            ViewBag.PopularCourses = data.PopularCourses;
            ViewBag.TopInstructors = data.TopInstructors;
            ViewBag.OrderChartData = data.OrderChartData;
            return View();
        }
    }
}