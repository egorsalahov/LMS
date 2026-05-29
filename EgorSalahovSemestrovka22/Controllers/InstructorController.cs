using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using EgorSalahovSemestrovka22.Models.Enums;
using EgorSalahovSemestrovka22.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;   
using Sem.Web.Services;

namespace EgorSalahovSemestrovka22.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly InstructorService _instructorService;
        private readonly UserManager<Student> _userManager;

        public InstructorController(InstructorService instructorService, UserManager<Student> userManager)
        {
            _instructorService = instructorService;
            _userManager = userManager;
        }

        private async Task<Student?> GetCurrentUserAsync()
            => await _userManager.GetUserAsync(User);

        private string GetInstructorDisplayName(Instructor? i, Student? u)
            => i != null ? $"{i.FirstName} {i.LastName}" : u?.Email ?? "Instructor";

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var instructor = await _instructorService.GetByEmailWithCoursesAsync(user.Email);

            if (instructor == null)
            {
                ViewBag.TotalStudents = 0;
                ViewBag.TotalCourses = 0;
                ViewBag.TotalEarnings = 0m;
                ViewBag.InstructorName = user.Email;
                return View(new List<Course>());
            }

            var (students, courses, earnings) = _instructorService.CalculateDashboard(instructor.Courses.ToList());
            ViewBag.TotalStudents = students;
            ViewBag.TotalCourses = courses;
            ViewBag.TotalEarnings = earnings;
            ViewBag.InstructorName = GetInstructorDisplayName(instructor, user);

            return View(instructor.Courses.OrderByDescending(c => c.Id).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var instructor = await _instructorService.GetByEmailAsync(user.Email)
                             ?? await _instructorService.CreateFromStudentAsync(user);

            ViewBag.InstructorName = GetInstructorDisplayName(instructor, user);
            return View(instructor);
        }

        [HttpPost]
        public async Task<IActionResult> EditFullProfile(EditInstructorFullViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            if (!ModelState.IsValid)
            {
                var instructor = await _instructorService.GetProfileForEditAsync(user.Email);
                ViewData["EditMode"] = true;
                return View("MyProfile", instructor);
            }

            var current = await _instructorService.GetProfileForEditAsync(user.Email)
                          ?? await _instructorService.CreateFromStudentAsync(user);

            await _instructorService.SaveProfileAsync(current, model);
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("MyProfile");
        }

        public async Task<IActionResult> Students()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var instructor = await _instructorService.GetByEmailAsync(user.Email);
            if (instructor == null) return View(new List<Student>());

            ViewBag.InstructorName = GetInstructorDisplayName(instructor, user);

            var students = await _instructorService.GetEnrolledStudentsAsync(instructor.Id);
            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> AddNewCourse()
        {
            var user = await GetCurrentUserAsync();
            var instructor = await _instructorService.GetByEmailAsync(user!.Email);
            var categories = await _instructorService.GetCategoriesForSelectListAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.InstructorName = GetInstructorDisplayName(instructor, user);
            return View(new CreateCourseViewModel());
        }

        [RequestFormLimits(MultipartBodyLengthLimit = 200 * 1024 * 1024)]
        [RequestSizeLimit(200 * 1024 * 1024)]
        [HttpPost]
        public async Task<IActionResult> AddNewCourse(CreateCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _instructorService.GetCategoriesForSelectListAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                ViewBag.ValidationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var instructor = await _instructorService.GetOrCreateInstructorAsync(user);
            await _instructorService.CreateCourseAsync(model, instructor.Id);

            TempData["SuccessMessage"] = "Курс успешно создан!";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Messages()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var instructor = await _instructorService.GetByEmailAsync(user.Email);
            if (instructor == null) return View();

            ViewBag.Contacts = await _instructorService.GetContactListAsync(instructor);
            ViewBag.CurrentUserId = user.Id;
            ViewBag.InstructorName = GetInstructorDisplayName(instructor, user);
            return View();
        }
    }
}