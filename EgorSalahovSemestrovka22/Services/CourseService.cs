using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Services
{
    public class CourseService
    {
        private readonly ICourseRepository _courseRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IWishlistRepository _wishlistRepo;

        public CourseService(
            ICourseRepository courseRepo,
            ICategoryRepository categoryRepo,
            IReviewRepository reviewRepo,
            IEnrollmentRepository enrollmentRepo,
            IWishlistRepository wishlistRepo)
        {
            _courseRepo = courseRepo;
            _categoryRepo = categoryRepo;
            _reviewRepo = reviewRepo;
            _enrollmentRepo = enrollmentRepo;
            _wishlistRepo = wishlistRepo;
        }

        public async Task<(List<Course> courses, int totalCount)> GetFilteredCoursesAsync(
            int? categoryId, string? search, Level? level,
            string? priceType, decimal? priceFrom, decimal? priceTo,
            int page, int pageSize)
        {
            var courses = await _courseRepo.GetFilteredAsync(categoryId, search, level, priceType, priceFrom, priceTo, page, pageSize);
            var totalCount = await _courseRepo.GetFilteredCountAsync(categoryId, search, level, priceType, priceFrom, priceTo);
            return (courses, totalCount);
        }

        public async Task<Course?> GetCourseDetailAsync(int id)
            => await _courseRepo.GetDetailByIdAsync(id);

        public async Task<List<Category>> GetAllCategoriesAsync()
            => await _categoryRepo.GetAllWithCoursesAsync();

        public async Task<List<object>> GetCategoryListAsync()
            => await _categoryRepo.GetCategoryListAsync();

        public async Task<Category?> GetCategoryWithCoursesAsync(int id)
            => await _categoryRepo.GetWithCoursesAsync(id);

        public async Task<List<Course>> GetPopularCoursesAsync(int count)
            => await _courseRepo.GetPopularAsync(count);

        public async Task<List<object>> SearchCoursesAsync(string query, int? categoryId)
        {
            var courses = await _courseRepo.SearchAsync(query, categoryId, 8);

            var result = courses.Select(c => new
            {
                id = c.Id,
                title = c.Title,
                imagePath = string.IsNullOrEmpty(c.ImagePath) || !c.ImagePath.StartsWith("/")
                            ? "/img/default.jpg" : c.ImagePath,
                instructor = c.Instructor != null
                            ? $"{c.Instructor.FirstName} {c.Instructor.LastName}"
                            : "Unknown",
                price = c.Price
            }).ToList<object>();

            return result;
        }

        public async Task<int> GetInstructorCourseCountAsync(int instructorId)
            => await _courseRepo.GetCountByInstructorAsync(instructorId);

        public async Task<(bool success, string message, double avgRating, int count)> RateCourseAsync(
            string userId, int courseId, int rating, bool isStudent)
        {
            if (rating < 1 || rating > 5)
                return (false, "Invalid rating", 0, 0);

            if (!isStudent)
                return (false, "Only enrolled students can rate", 0, 0);

            var enrolled = await _enrollmentRepo.AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);
            if (!enrolled)
                return (false, "You must purchase the course first", 0, 0);

            var review = await _reviewRepo.FindByStudentAndCourseAsync(userId, courseId);
            if (review == null)
            {
                review = new Review { StudentId = userId, CourseId = courseId, Rating = rating, CreatedAt = DateTime.Now };
                await _reviewRepo.AddAsync(review);
            }
            else
            {
                review.Rating = rating;
                _reviewRepo.Update(review);
            }
            await _reviewRepo.SaveChangesAsync();

            var avg = await _reviewRepo.GetAverageRatingAsync(courseId);
            var count = await _reviewRepo.GetReviewCountAsync(courseId);

            return (true, "Rated", avg, count);
        }

        // В CourseService.cs
        public async Task<object> GetFilteredCoursesAjaxAsync(
            int? categoryId, string? search, Level? level,
            string? priceType, decimal? priceFrom, decimal? priceTo,
            int page, int pageSize)
        {
            var courses = await _courseRepo.GetFilteredAsync(
                categoryId, search, level, priceType, priceFrom, priceTo, page, pageSize);

            var totalCount = await _courseRepo.GetFilteredCountAsync(
                categoryId, search, level, priceType, priceFrom, priceTo);

            var result = courses.Select(c => new
            {
                id = c.Id,
                title = c.Title,
                shortDescription = c.ShortDescription,
                price = c.Price,
                oldPrice = c.OldPrice,
                imagePath = string.IsNullOrEmpty(c.ImagePath) || !c.ImagePath.StartsWith("/")
                            ? "/img/default.jpg" : c.ImagePath,
                instructorName = c.Instructor != null ? $"{c.Instructor.FirstName} {c.Instructor.LastName}" : "Unknown",
                categoryName = c.Category?.Name ?? "Uncategorized",
                avgRating = c.Reviews.Any() ? Math.Round(c.Reviews.Average(r => r.Rating), 1) : 0.0,
                reviewCount = c.Reviews.Count,
                levelForStudent = c.LevelForStudent.ToString()
            }).ToList();

            return new
            {
                courses = result,
                currentPage = page,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                totalCourses = totalCount,
                pageSize
            };
        }



        public async Task<List<int>> GetWishlistCourseIdsAsync(string userId)
            => await _wishlistRepo.GetCourseIdsAsync(userId);
    }
}
