using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Repositories
{
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(AppDbContext context) : base(context) { }

        public async Task<List<Wishlist>> GetByStudentAsync(string studentId)
            => await _dbSet
                .Include(w => w.Course).ThenInclude(c => c.Instructor)
                .Include(w => w.Course).ThenInclude(c => c.Reviews)
                .Include(w => w.Course).ThenInclude(c => c.Category)
                .Where(w => w.StudentId == studentId)
                .OrderByDescending(w => w.Id)
                .ToListAsync();

        public async Task<List<int>> GetCourseIdsAsync(string studentId)
            => await _dbSet.Where(w => w.StudentId == studentId).Select(w => w.CourseId).ToListAsync();

        public async Task<Wishlist?> FindByStudentAndCourseAsync(string studentId, int courseId)
            => await _dbSet.FirstOrDefaultAsync(w => w.StudentId == studentId && w.CourseId == courseId);
    }
}
