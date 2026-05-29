using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sem.Web.Services;

namespace EgorSalahovSemestrovka22.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly UserManager<Student> _userManager;
        private readonly StudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(UserManager<Student> userManager, StudentService studentService, ILogger<StudentController> logger)
        {
            _userManager = userManager;
            _studentService = studentService;
            _logger = logger;
        }

        private async Task<Student?> GetCurrentUserAsync()
            => await _userManager.GetUserAsync(User);

        private string GetUserId() => _userManager.GetUserId(User)!;

        public async Task<IActionResult> MyProfile()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                if (user == null) return RedirectToAction("SignIn", "Account");
                ViewData["EditMode"] = true;
                return View("MyProfile", user);
            }

            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return RedirectToAction("SignIn", "Account");

            _logger.LogInformation("Редактирование профиля студента {Email}", currentUser.Email);
            await _studentService.EditProfileAsync(currentUser, model);
            TempData["SuccessMessage"] = "Профиль успешно обновлён!";
            return RedirectToAction("MyProfile");
        }

        public async Task<IActionResult> Courses()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var enrollments = await _studentService.GetEnrollmentsAsync(user.Id);
            return View(enrollments);
        }

        public async Task<IActionResult> Wishlist()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var items = await _studentService.GetWishlistAsync(user.Id);
            return View(items);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddToWishlist(int courseId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Json(new { success = false, message = "Not authorized" });

            if (await _userManager.IsInRoleAsync(user, "Instructor"))
                return Json(new { success = false, message = "Instructors cannot add to wishlist" });

            if (await _studentService.IsInWishlistAsync(user.Id, courseId))
                return Json(new { success = false, message = "Already in wishlist" });

            _logger.LogInformation("Добавление в wishlist: студент={Email}, курс={CourseId}", user.Email, courseId);
            await _studentService.AddToWishlistAsync(user.Id, courseId);
            return Json(new { success = true, message = "Added to wishlist" });
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> RemoveFromWishlist(int courseId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            _logger.LogInformation("Удаление из wishlist: студент={Email}, курс={CourseId}", user.Email, courseId);
            await _studentService.RemoveFromWishlistAsync(user.Id, courseId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction("Wishlist");
        }

        public async Task<IActionResult> OrderHistory()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var orders = await _studentService.GetOrderHistoryAsync(user.Id);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            var order = await _studentService.GetOrderDetailAsync(user.Id, orderId);
            if (order == null) return NotFound();

            return PartialView("_OrderDetailPartial", order);
        }

        public IActionResult Settings() => View();
        public IActionResult BecomeInstructor() => View();

        [HttpGet]
        public async Task<IActionResult> WatchCourse(int enrollmentId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var enrollment = await _studentService.GetEnrollmentAsync(user.Id, enrollmentId);
            if (enrollment == null) return NotFound("Enrollment not found or access denied");

            ViewBag.UserRating = _studentService.GetUserRating(enrollment, user.Id) ?? 0;
            return View(enrollment);
        }

        [HttpGet]
        public async Task<IActionResult> WatchLesson(int enrollmentId, int lessonId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var enrollment = await _studentService.GetEnrollmentForLessonAsync(user.Id, enrollmentId);
            if (enrollment == null) return NotFound("Enrollment not found or access denied");

            var lesson = await _studentService.GetLessonAsync(lessonId, enrollment.CourseId);
            if (lesson == null) return NotFound("Lesson not found");

            ViewBag.CourseTitle = enrollment.Course.Title;
            ViewBag.EnrollmentId = enrollmentId;

            return View(lesson);
        }

        [HttpGet]
        public async Task<IActionResult> Messages()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            ViewBag.Contacts = await _studentService.GetChatContactsAsync(user.Id);
            ViewBag.CurrentUserId = user.Id;

            return View();
        }
    }
}