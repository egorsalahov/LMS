using EgorSalahovSemestrovka22.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Areas.Admin.Services;

namespace EgorSalahovSemestrovka22.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DataAdminController : Controller
    {
        private readonly AdminService _adminService;

        public DataAdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> InstructorsData()
        {
            var instructors = await _adminService.GetAllInstructorsAsync();
            return View(instructors);
        }

        public async Task<IActionResult> StudentsData()
        {
            var students = await _adminService.GetAllStudentsAsync();
            return View(students);
        }
    }
}
