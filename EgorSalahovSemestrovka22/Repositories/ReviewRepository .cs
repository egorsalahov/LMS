using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Sem.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Sem.Web.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context) { }

        public async Task<Review?> FindByStudentAndCourseAsync(string studentId, int courseId)
            => await _dbSet.FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseId == courseId);

        public async Task<double> GetAverageRatingAsync(int courseId)
            => await _dbSet.Where(r => r.CourseId == courseId).AverageAsync(r => (double?)r.Rating) ?? 0;

        public async Task<int> GetReviewCountAsync(int courseId)
            => await _dbSet.CountAsync(r => r.CourseId == courseId);
    }
}
