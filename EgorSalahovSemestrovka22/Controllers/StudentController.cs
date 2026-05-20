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
        // GET: /Student/Wishlist
        public async Task<IActionResult> Wishlist()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var wishlistItems = await _context.Wishlists
                .Include(w => w.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(w => w.Course)
                    .ThenInclude(c => c.Reviews)
                .Include(w => w.Course)
                    .ThenInclude(c => c.Category)
                .Where(w => w.StudentId == user.Id)
                .OrderByDescending(w => w.Id) // или по дате, если добавите
                .ToListAsync();

            return View(wishlistItems);
        }

        // POST: /Student/AddToWishlist?courseId=5 (AJAX)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWishlist(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Not authorized" });

            // Проверяем, нет ли уже в вишлисте
            var alreadyInWishlist = await _context.Wishlists
                .AnyAsync(w => w.StudentId == user.Id && w.CourseId == courseId);
            if (alreadyInWishlist)
                return Json(new { success = false, message = "Already in wishlist" });

            var wishlistItem = new Wishlist
            {
                StudentId = user.Id,
                CourseId = courseId
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Added to wishlist" });
        }

        // POST: /Student/RemoveFromWishlist?courseId=5 (AJAX или форма)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

            var item = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.StudentId == user.Id && w.CourseId == courseId);
            if (item != null)
            {
                _context.Wishlists.Remove(item);
                await _context.SaveChangesAsync();
            }

            // Если вызов был с формы на странице Wishlist — вернёмся обратно, иначе JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction("Wishlist");
        }
        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .Where(o => o.StudentId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.StudentId == user.Id);

            if (order == null) return NotFound();

            return PartialView("_OrderDetailPartial", order);
        }
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
