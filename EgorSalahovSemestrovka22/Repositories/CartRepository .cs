using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Repositories
{
    public class CartRepository : Repository<CartItem>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<List<CartItem>> GetByStudentAsync(string studentId)
            => await _dbSet
                .Include(c => c.Course).ThenInclude(c => c.Instructor)
                .Include(c => c.Course).ThenInclude(c => c.Reviews)
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.AddedAt)
                .ToListAsync();

        public async Task<int> GetCountByStudentAsync(string studentId)
            => await _dbSet.CountAsync(c => c.StudentId == studentId);

        public async Task<CartItem?> GetByIdAndStudentAsync(int id, string studentId)
            => await _dbSet.FirstOrDefaultAsync(c => c.Id == id && c.StudentId == studentId);

        public async Task<List<CartItem>> GetAllByStudentAsync(string studentId)
    => await _dbSet
        .Include(c => c.Course)           
        .ThenInclude(c => c.Instructor)  
        .Where(c => c.StudentId == studentId)
        .ToListAsync();
    }
}
