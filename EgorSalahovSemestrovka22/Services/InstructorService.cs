using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using EgorSalahovSemestrovka22.Models.Enums;
using EgorSalahovSemestrovka22.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Services
{
    public class InstructorService
    {
        private readonly IInstructorRepository _instructorRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ICourseRepository _courseRepo;

        public InstructorService(
            IInstructorRepository instructorRepo,
            IEnrollmentRepository enrollmentRepo,
            ICategoryRepository categoryRepo,
            ICourseRepository courseRepo)
        {
            _instructorRepo = instructorRepo;
            _enrollmentRepo = enrollmentRepo;
            _categoryRepo = categoryRepo;
            _courseRepo = courseRepo;
        }

        public async Task<Instructor?> GetByEmailWithCoursesAsync(string email)
            => await _instructorRepo.GetByEmailWithCoursesAsync(email);

        public async Task<Instructor> CreateFromStudentAsync(Student user)
        {
            var instructor = new Instructor
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
            await _instructorRepo.AddAsync(instructor);
            await _instructorRepo.SaveChangesAsync();
            return instructor;
        }

        public (int totalStudents, int totalCourses, decimal totalEarnings) CalculateDashboard(List<Course> courses)
        {
            var totalStudents = courses.Sum(c => c.Enrollments?.Count ?? 0);
            var totalCourses = courses.Count;
            var totalEarnings = courses.Sum(c => c.Price * (c.Enrollments?.Count ?? 0));
            return (totalStudents, totalCourses, totalEarnings);
        }

        public async Task<List<Student>> GetEnrolledStudentsAsync(int instructorId)
            => await _enrollmentRepo.GetInstructorStudentsAsync(instructorId);

        public async Task<Instructor?> GetProfileForEditAsync(string email)
            => await _instructorRepo.GetByEmailWithProfileAsync(email);

        public async Task SaveProfileAsync(Instructor instructor, EditInstructorFullViewModel model)
        {
            instructor.FirstName = model.FirstName;
            instructor.LastName = model.LastName;
            instructor.Gender = model.Gender;
            instructor.PhoneNumber = model.PhoneNumber;
            instructor.DateOfBirth = model.DateOfBirth;
            instructor.Bio = model.Bio;

            instructor.Educations = model.Educations?
                .Where(e => !string.IsNullOrWhiteSpace(e.Degree) || !string.IsNullOrWhiteSpace(e.Institute))
                .Select(e => new Education { Degree = e.Degree ?? "", Institute = e.Institute ?? "", Years = e.Years ?? "" })
                .ToList() ?? new List<Education>();

            instructor.Experiences = model.Experiences?
                .Where(e => !string.IsNullOrWhiteSpace(e.Position) || !string.IsNullOrWhiteSpace(e.Company))
                .Select(e => new Experience { Position = e.Position ?? "", Company = e.Company ?? "", Years = e.Years ?? "" })
                .ToList() ?? new List<Experience>();

            _instructorRepo.Update(instructor);
            await _instructorRepo.SaveChangesAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
            => await _categoryRepo.GetAllAsync();

        public async Task<Instructor> GetOrCreateInstructorAsync(Student user)
            => await _instructorRepo.GetByEmailAsync(user.Email) ?? await CreateFromStudentAsync(user);

        public async Task<Course> CreateCourseAsync(CreateCourseViewModel model, int instructorId)
        {
            var course = new Course
            {
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                FullDescription = model.FullDescription,
                CategoryId = model.CategoryId,
                LevelForStudent = model.LevelForStudent ?? Level.Beginner,
                InstructorId = instructorId,
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
                HasSubtitles = model.HasSubtitles,
                Sections = new List<Section>() // ← Инициализация коллекции
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
                course.ImagePath = await SaveFileAsync(model.ImageFile, "wwwroot/uploads/courses", "/uploads/courses");

            await _courseRepo.AddAsync(course);
            await _courseRepo.SaveChangesAsync();

            if (model.Sections != null)
            {
                foreach (var sectionVm in model.Sections)
                {
                    if (string.IsNullOrWhiteSpace(sectionVm.Title)) continue;

                    var section = new Section
                    {
                        Title = sectionVm.Title,
                        CourseId = course.Id,
                        Lessons = new List<Lesson>() // ← Инициализация коллекции уроков
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

                            if (lessonVm.VideoFile != null && lessonVm.VideoFile.Length > 0)
                                lesson.VideoLink = await SaveFileAsync(lessonVm.VideoFile, "wwwroot/uploads/videos", "/uploads/videos");

                            section.Lessons.Add(lesson); // ← Теперь не упадет
                        }
                    }

                    course.Sections.Add(section);
                }

                await _courseRepo.SaveChangesAsync();
            }

            return course;
        }

        public async Task<List<object>> GetContactListAsync(Instructor instructor)
        {
            var students = await _enrollmentRepo.GetInstructorStudentsAsync(instructor.Id);
            return students.Select(s => new { Id = s.Id, DisplayName = $"{s.FirstName} {s.LastName}" }).ToList<object>();
        }

        public async Task<Instructor?> GetByEmailAsync(string email)
            => await _instructorRepo.GetByEmailAsync(email);
        public async Task<List<Category>> GetCategoriesForSelectListAsync()
            => await _categoryRepo.GetAllAsync();

        private async Task<string> SaveFileAsync(IFormFile file, string folder, string urlPrefix)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var uniqueName = Guid.NewGuid() + "_" + Path.GetFileNameWithoutExtension(file.FileName)
                .Replace("(", "").Replace(")", "").Replace(";", "").Replace(",", "").Replace(" ", "_")
                + Path.GetExtension(file.FileName);

            var fullPath = Path.Combine(folderPath, uniqueName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            return urlPrefix + "/" + uniqueName;
        }
    }
}
