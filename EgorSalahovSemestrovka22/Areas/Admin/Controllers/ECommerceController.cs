using EgorSalahovSemestrovka22.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Areas.Admin.Services;

namespace EgorSalahovSemestrovka22.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ECommerceController : Controller
    {
        private readonly AdminService _adminService;

        public ECommerceController(AdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Products()
        {
            var courses = await _adminService.GetAllCoursesAsync();
            return View(courses);
        }

        public async Task<IActionResult> Customers()
        {
            var customers = await _adminService.GetCustomersAsync();
            return View(customers);
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _adminService.GetAllOrdersAsync();
            return View(orders);
        }
    }
}