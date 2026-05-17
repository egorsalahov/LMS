using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using EgorSalahovSemestrovka22.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Student> _userManager;

        public InstructorController(AppDbContext context, UserManager<Student> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Instructor/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

            var instructor = await _context.Instructors
                .Include(i => i.Courses)
                .FirstOrDefaultAsync(i => i.Email == user.Email);

            ViewBag.CoursesCount = instructor?.Courses?.Count ?? 0;
            ViewBag.InstructorName = instructor?.FirstName ?? user.Email;

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View();
        }

        // GET: /Instructor/MyProfile
        [HttpGet]
        public IActionResult MyProfile()
        {
            return View();
        }

        // GET: /Instructor/Courses
        [HttpGet]
        public async Task<IActionResult> Courses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.Email == user.Email);

            if (instructor == null)
                return View(new List<Course>());

            var courses = await _context.Courses
                .Include(c => c.Category)
                .Where(c => c.InstructorId == instructor.Id)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(courses);
        }

        // GET: /Instructor/Students
        [HttpGet]
        public IActionResult Students()
        {
            return View();
        }

        // GET: /Instructor/Settings
        [HttpGet]
        public IActionResult Settings()
        {
            return View();
        }

        // GET: /Instructor/AddNewCourse
        [HttpGet]
        public async Task<IActionResult> AddNewCourse()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(new CreateCourseViewModel());
        }

        // POST: /Instructor/AddNewCourse
        [HttpPost]
        public async Task<IActionResult> AddNewCourse(CreateCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _context.Categories.ToListAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

            // Находим или создаем запись Instructor
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Email == user.Email);
            if (instructor == null)
            {
                instructor = new Instructor
                {
                    FirstName = user.FirstName ?? "Instructor",
                    LastName = user.LastName ?? "",
                    UserName = user.UserName ?? user.Email,
                    Email = user.Email,
                    Bio = user.Bio ?? "",
                    AvatarPath = user.AvatarPath ?? "instructor-default.png",
                    RegistrationDate = user.RegistrationDate,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    TotalEarnings = 0
                };
                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();
            }

            var course = new Course
            {
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                FullDescription = model.FullDescription,
                CategoryId = model.CategoryId,
                LevelForStudent = model.LevelForStudent,
                InstructorId = instructor.Id,
                Price = model.IsFree ? 0 : (model.Price ?? 0),
                OldPrice = model.IsFree ? null : model.OldPrice,
                Duration = TimeSpan.Zero,
                LessonsCount = 0,
                ImagePath = "/img/course/default.jpg",
                HasLifetimeAccess = true,
                HasMobileAccess = true,
                HasAssignments = false,
                HasCommunityAccess = false,
                HasDownloadableResources = false,
                HasSubtitles = false
            };

            // Загрузка изображения
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/courses");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                course.ImagePath = "/uploads/courses/" + uniqueFileName;
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Курс успешно создан!";
            return RedirectToAction("Dashboard");
        }
    }
}