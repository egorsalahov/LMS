using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Services
{
    public class HomeService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IInstructorRepository _instructorRepo;

        public HomeService(
            ICategoryRepository categoryRepo,
            ICourseRepository courseRepo,
            IInstructorRepository instructorRepo)
        {
            _categoryRepo = categoryRepo;
            _courseRepo = courseRepo;
            _instructorRepo = instructorRepo;
        }

        public async Task<List<Category>> GetCategoriesWithCoursesAsync()
            => await _categoryRepo.GetAllWithCoursesAsync();

        public async Task<List<Course>> GetPopularCoursesAsync(int count = 6)
            => await _courseRepo.GetPopularAsync(count);

        public async Task<int> GetInstructorCountAsync()
            => await _instructorRepo.GetTotalCountAsync();

        public async Task<int> GetStudentCountAsync()
            => await _courseRepo.CountAsync(); // Users count — оставим через CourseService или добавим IUserRepository

        public async Task<int> GetCourseCountAsync()
            => await _courseRepo.CountAsync();
    }
}
