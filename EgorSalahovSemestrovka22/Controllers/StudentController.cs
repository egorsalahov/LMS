using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly UserManager<Student> _userManager;
        private readonly AppDbContext _context;

        public StudentController(UserManager<Student> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Вернёмся на страницу профиля с ошибками
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("SignIn", "Account");
                // Передадим модель с ошибками, а также самого user для отображения (необязательно)
                ViewData["EditMode"] = true; // чтобы форма была показана
                return View("MyProfile", user);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("SignIn", "Account");

            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;
            currentUser.Gender = model.Gender;
            currentUser.PhoneNumber = model.PhoneNumber;
            currentUser.DateOfBirth = model.DateOfBirth;
            currentUser.Bio = model.Bio;

            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Профиль успешно обновлён!";
                return RedirectToAction("MyProfile");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View("MyProfile", currentUser);
        }
        public async Task<IActionResult> Courses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Category)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Reviews)
                .Where(e => e.StudentId == user.Id)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            return View(enrollments);
        }
        public IActionResult Wishlist() => View();
        public IActionResult OrderHistory() => View();
        public IActionResult Settings() => View();
        public IActionResult BecomeInstructor() => View();

        // GET: /Student/WatchCourse/{enrollmentId}
        [HttpGet]
        public async Task<IActionResult> WatchCourse(int enrollmentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Sections)
                        .ThenInclude(s => s.Lessons)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.StudentId == user.Id);

            if (enrollment == null)
                return NotFound("Enrollment not found or access denied");

            return View(enrollment);
        }

        // GET: /Student/WatchLesson?enrollmentId=5&lessonId=12
        [HttpGet]
        public async Task<IActionResult> WatchLesson(int enrollmentId, int lessonId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            // Проверяем, что студент записан на этот курс
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.StudentId == user.Id);

            if (enrollment == null)
                return NotFound("Enrollment not found or access denied");

            // Загружаем урок (должен принадлежать курсу этой записи)
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                .FirstOrDefaultAsync(l => l.Id == lessonId && l.Section.CourseId == enrollment.CourseId);

            if (lesson == null)
                return NotFound("Lesson not found");

            ViewBag.CourseTitle = enrollment.Course.Title;
            ViewBag.EnrollmentId = enrollmentId;

            return View(lesson);
        }
    }
}
