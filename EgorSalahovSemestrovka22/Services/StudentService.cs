using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Orders;
using EgorSalahovSemestrovka22.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Sem.Web.Services
{
    public class StudentService
    {
        private readonly IRepository<Student> _userRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IWishlistRepository _wishlistRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            IRepository<Student> userRepo,
            IEnrollmentRepository enrollmentRepo,
            IWishlistRepository wishlistRepo,
            IOrderRepository orderRepo,
            IReviewRepository reviewRepo,
            ILessonRepository lessonRepo,
            ILogger<StudentService> logger)
        {
            _userRepo = userRepo;
            _enrollmentRepo = enrollmentRepo;
            _wishlistRepo = wishlistRepo;
            _orderRepo = orderRepo;
            _lessonRepo = lessonRepo;
            _logger = logger;
        }

        public async Task<Student?> GetProfileAsync(string userId)
            => await _userRepo.GetByIdAsync(userId);

        public async Task EditProfileAsync(Student user, EditProfileViewModel model)
        {
            _logger.LogInformation("Обновление профиля студента {Email}", user.Email);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            user.Bio = model.Bio;
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();
        }

        public async Task<List<Enrollment>> GetEnrollmentsAsync(string userId)
            => await _enrollmentRepo.GetStudentEnrollmentsAsync(userId);

        public async Task<List<Wishlist>> GetWishlistAsync(string userId)
            => await _wishlistRepo.GetByStudentAsync(userId);

        public async Task<bool> IsInWishlistAsync(string userId, int courseId)
            => await _wishlistRepo.AnyAsync(w => w.StudentId == userId && w.CourseId == courseId);

        public async Task AddToWishlistAsync(string userId, int courseId)
        {
            _logger.LogInformation("Добавление в wishlist: студент={User}, курс={Course}", userId, courseId);
            await _wishlistRepo.AddAsync(new Wishlist { StudentId = userId, CourseId = courseId });
            await _wishlistRepo.SaveChangesAsync();
        }

        public async Task RemoveFromWishlistAsync(string userId, int courseId)
        {
            _logger.LogInformation("Удаление из wishlist: студент={User}, курс={Course}", userId, courseId);
            var item = await _wishlistRepo.FindByStudentAndCourseAsync(userId, courseId);
            if (item != null)
            {
                _wishlistRepo.Delete(item);
                await _wishlistRepo.SaveChangesAsync();
            }
        }

        public async Task<List<Order>> GetOrderHistoryAsync(string userId)
            => await _orderRepo.GetByStudentAsync(userId);

        public async Task<Order?> GetOrderDetailAsync(string userId, int orderId)
            => await _orderRepo.GetByIdAndStudentAsync(orderId, userId);

        public async Task<Enrollment?> GetEnrollmentAsync(string userId, int enrollmentId)
            => await _enrollmentRepo.GetByIdWithCourseAsync(userId, enrollmentId);

        public async Task<Enrollment?> GetEnrollmentForLessonAsync(string userId, int enrollmentId)
            => await _enrollmentRepo.GetByIdForLessonAsync(userId, enrollmentId);

        public int? GetUserRating(Enrollment enrollment, string userId)
            => enrollment.Course.Reviews?.FirstOrDefault(r => r.StudentId == userId)?.Rating;

        public async Task<List<dynamic>> GetChatContactsAsync(string userId)
        {
            var instructors = await _enrollmentRepo.GetStudentInstructorsAsync(userId);
            var contacts = new List<dynamic>();
            foreach (var instructor in instructors)
            {
                var instructorUser = await _userRepo.FirstOrDefaultAsync(u => u.Email == instructor.Email);
                if (instructorUser != null)
                    contacts.Add(new
                    {
                        Id = instructorUser.Id,
                        DisplayName = $"{instructor.FirstName} {instructor.LastName}"
                    });
            }
            return contacts;
        }

        public async Task<Lesson?> GetLessonAsync(int lessonId, int courseId)
        {
            var lesson = await _lessonRepo.GetByIdWithSectionAsync(lessonId);
            if (lesson == null || lesson.Section.CourseId != courseId)
                return null;
            return lesson;
        }
    }
}