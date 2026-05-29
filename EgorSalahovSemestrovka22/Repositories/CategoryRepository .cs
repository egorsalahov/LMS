using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<List<Category>> GetAllWithCoursesAsync()
            => await _dbSet.Include(c => c.Courses).OrderBy(c => c.Name).ToListAsync();

        public async Task<List<object>> GetCategoryListAsync()
            => await _dbSet.Select(cat => new { cat.Id, cat.Name, Count = cat.Courses.Count })
                .ToListAsync<object>();

        public async Task<Category?> GetWithCoursesAsync(int id)
            => await _dbSet
                .Include(c => c.Courses).ThenInclude(c => c.Instructor)
                .Include(c => c.Courses).ThenInclude(c => c.Reviews)
                .Include(c => c.Courses).ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);
    }
}
