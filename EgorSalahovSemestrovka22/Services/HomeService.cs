using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Services
{
    public class HomeService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly UserManager<Student> _userManager;
        private readonly ILogger<HomeService> _logger;

        public HomeService(
            ICategoryRepository categoryRepo,
            ICourseRepository courseRepo,
            IInstructorRepository instructorRepo,
            ILogger<HomeService> logger,
            UserManager<Student> userManager)
        {
            _categoryRepo = categoryRepo;
            _courseRepo = courseRepo;
            _instructorRepo = instructorRepo;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<List<Category>> GetCategoriesWithCoursesAsync()
            => await _categoryRepo.GetAllWithCoursesAsync();

        public async Task<List<Course>> GetPopularCoursesAsync(int count = 6)
            => await _courseRepo.GetPopularAsync(count);

        public async Task<int> GetInstructorCountAsync()
            => await _instructorRepo.GetTotalCountAsync();

        public async Task<int> GetStudentCountAsync()
            => await _userManager.Users.CountAsync();

        public async Task<int> GetCourseCountAsync()
            => await _courseRepo.CountAsync();
    }
}