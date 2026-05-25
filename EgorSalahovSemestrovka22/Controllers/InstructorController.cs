using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using EgorSalahovSemestrovka22.Models.Enums;
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
                    .ThenInclude(c => c.Enrollments)
                      .Include(i => i.Courses)
                        .ThenInclude(c => c.Reviews)
                .FirstOrDefaultAsync(i => i.Email == user.Email);

            if (instructor == null)
            {
                // Инструктор ещё не создан в таблице Instructors
                ViewBag.TotalStudents = 0;
                ViewBag.TotalCourses = 0;
                ViewBag.TotalEarnings = 0m;
                ViewBag.InstructorName = user.Email;
                return View(new List<Course>());
            }

            var courses = instructor.Courses.ToList();
            var totalStudents = courses.Sum(c => c.Enrollments?.Count ?? 0);
            var totalCourses = courses.Count;
            var totalEarnings = courses.Sum(c => c.Price * (c.Enrollments?.Count ?? 0));

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.TotalEarnings = totalEarnings;
            ViewBag.InstructorName = instructor.FirstName ?? user.Email;

            return View(courses.OrderByDescending(c => c.Id).ToList());
        }

        // GET: /Instructor/MyProfile
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            // Загружаем инструктора с Education и Experience
            var instructor = await _context.Instructors
                .Include(i => i.Educations)
                .Include(i => i.Experiences)
                .FirstOrDefaultAsync(i => i.Email == user.Email);

            if (instructor == null)
            {
                // Если запись инструктора еще не создана – создаём
                instructor = new Instructor
                {
                    FirstName = user.FirstName ?? "Instructor",
                    LastName = user.LastName ?? "",
                    UserName = user.UserName ?? user.Email,
                    Email = user.Email,
                    Bio = user.Bio ?? "",
                    AvatarPath = user.AvatarPath ?? "",
                    RegistrationDate = user.RegistrationDate,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    TotalEarnings = 0
                };
                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();
            }

            return View(instructor);
        }

        // GET: /Instructor/Students
        [HttpGet]
        public async Task<IActionResult> Students()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

            var instructor = await _context.Instructors
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(i => i.Email == user.Email);

            if (instructor == null)
                return View(new List<Student>());

            // Все уникальные студенты, записанные на курсы этого инструктора
            var students = instructor.Courses
                .SelectMany(c => c.Enrollments ?? Enumerable.Empty<Enrollment>())
                .Select(e => e.Student)
                .Where(s => s != null)
                .DistinctBy(s => s.Id)
                .OrderBy(s => s.FirstName)
                .ToList();

            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> EditFullProfile(EditInstructorFullViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("SignIn", "Account");

                var instructor = await _context.Instructors
                    .Include(i => i.Educations)
                    .Include(i => i.Experiences)
                    .FirstOrDefaultAsync(i => i.Email == user.Email);

                ViewData["EditMode"] = true;
                return View("MyProfile", instructor);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("SignIn", "Account");

            var currentInstructor = await _context.Instructors
                .Include(i => i.Educations)
                .Include(i => i.Experiences)
                .FirstOrDefaultAsync(i => i.Email == currentUser.Email);

            if (currentInstructor == null)
            {
                currentInstructor = new Instructor
                {
                    Email = currentUser.Email,
                    UserName = currentUser.UserName ?? currentUser.Email,
                    FirstName = currentUser.FirstName ?? "",
                    LastName = currentUser.LastName ?? "",
                    RegistrationDate = currentUser.RegistrationDate,
                    DateOfBirth = currentUser.DateOfBirth,
                    Gender = currentUser.Gender ?? "",
                    PhoneNumber = currentUser.PhoneNumber ?? ""
                };
                _context.Instructors.Add(currentInstructor);
                await _context.SaveChangesAsync();
            }

            // Обновляем базовые поля
            currentInstructor.FirstName = model.FirstName;
            currentInstructor.LastName = model.LastName;
            currentInstructor.Gender = model.Gender;
            currentInstructor.PhoneNumber = model.PhoneNumber;
            currentInstructor.DateOfBirth = model.DateOfBirth;
            currentInstructor.Bio = model.Bio;

            // Обновляем Education
            _context.Educations.RemoveRange(currentInstructor.Educations);
            if (model.Educations != null)
            {
                currentInstructor.Educations = model.Educations
                    .Where(e => !string.IsNullOrWhiteSpace(e.Degree) || !string.IsNullOrWhiteSpace(e.Institute))
                    .Select(e => new Education
                    {
                        Degree = e.Degree ?? "",
                        Institute = e.Institute ?? "",
                        Years = e.Years ?? "",
                        InstructorId = currentInstructor.Id
                    }).ToList();
            }

            // Обновляем Experience
            _context.Experiences.RemoveRange(currentInstructor.Experiences);
            if (model.Experiences != null)
            {
                currentInstructor.Experiences = model.Experiences
                    .Where(e => !string.IsNullOrWhiteSpace(e.Position) || !string.IsNullOrWhiteSpace(e.Company))
                    .Select(e => new Experience
                    {
                        Position = e.Position ?? "",
                        Company = e.Company ?? "",
                        Years = e.Years ?? "",
                        InstructorId = currentInstructor.Id
                    }).ToList();
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("MyProfile");
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
            // Временная отладка
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            System.Diagnostics.Debug.WriteLine("ModelState errors: " + string.Join(" | ", errors));

            if (!ModelState.IsValid)
            {
                var categories = await _context.Categories.ToListAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");

                // Собираем ошибки для клиентской модалки
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                ViewBag.ValidationErrors = errorMessages;

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

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
                LevelForStudent = model.LevelForStudent ?? Level.Beginner,
                InstructorId = instructor.Id,
                Price = model.IsFree ? 0 : (model.Price ?? 0),
                OldPrice = model.IsFree ? null : model.OldPrice,
                Duration = TimeSpan.Zero,
                LessonsCount = model.Sections?.Sum(s => s.Lessons?.Count ?? 0) ?? 0,
                ImagePath = "/img/course/default.jpg",
                HasLifetimeAccess = model.HasLifetimeAccess,
                HasMobileAccess = model.HasMobileAccess,
                HasAssignments = model.HasAssignments,
                HasCommunityAccess = model.HasCommunityAccess,
                HasDownloadableResources = model.HasDownloadableResources,
                HasSubtitles = model.HasSubtitles
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/courses");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileNameWithoutExtension(model.ImageFile.FileName)
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(";", "")
                    .Replace(",", "")
                    .Replace(" ", "_")
                    + Path.GetExtension(model.ImageFile.FileName);

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                course.ImagePath = "/uploads/courses/" + uniqueFileName;
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            if (model.Sections != null)
            {
                foreach (var sectionVm in model.Sections)
                {
                    if (string.IsNullOrWhiteSpace(sectionVm.Title)) continue;

                    var section = new Section
                    {
                        Title = sectionVm.Title,
                        CourseId = course.Id,
                        Lessons = new List<Lesson>()
                    };

                    if (sectionVm.Lessons != null)
                    {
                        foreach (var lessonVm in sectionVm.Lessons)
                        {
                            if (string.IsNullOrWhiteSpace(lessonVm.Title)) continue;

                            var lesson = new Lesson
                            {
                                Title = lessonVm.Title,
                                Duration = "0:00",
                                IsPreview = false
                            };

                            // Сохранение видеофайла
                            if (lessonVm.VideoFile != null && lessonVm.VideoFile.Length > 0)
                            {
                                var videoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/videos");
                                if (!Directory.Exists(videoFolder))
                                    Directory.CreateDirectory(videoFolder);

                                var uniqueVideoName = Guid.NewGuid().ToString() + "_" +
                                    Path.GetFileNameWithoutExtension(lessonVm.VideoFile.FileName)
                                        .Replace("(", "").Replace(")", "").Replace(";", "").Replace(",", "").Replace(" ", "_")
                                    + Path.GetExtension(lessonVm.VideoFile.FileName);

                                var videoPath = Path.Combine(videoFolder, uniqueVideoName);
                                using (var stream = new FileStream(videoPath, FileMode.Create))
                                {
                                    await lessonVm.VideoFile.CopyToAsync(stream);
                                }
                                lesson.VideoLink = "/uploads/videos/" + uniqueVideoName;
                            }

                            section.Lessons.Add(lesson);
                        }
                    }

                    _context.Sections.Add(section);
                }

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Курс успешно создан!";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Messages()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Email == user.Email);
            if (instructor == null)
                return View(new List<Student>());

            var students = await _context.Enrollments
                .Where(e => e.Course.InstructorId == instructor.Id)
                .Select(e => e.Student)
                .Distinct()
                .ToListAsync();

            var contacts = students.Select(s => new
            {
                Id = s.Id,
                DisplayName = $"{s.FirstName} {s.LastName}"
            }).ToList();

            ViewBag.Contacts = contacts;
            ViewBag.CurrentUserId = user.Id;

            return View();
        }
    }
}