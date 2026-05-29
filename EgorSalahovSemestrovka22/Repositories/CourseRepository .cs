using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context) { }

        private IQueryable<Course> BaseQuery()
            => _dbSet
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .AsQueryable();

        public async Task<List<Course>> GetFilteredAsync(
            int? categoryId, string? search, Level? level,
            string? priceType, decimal? priceFrom, decimal? priceTo,
            int page, int pageSize)
        {
            var query = ApplyFilters(BaseQuery(), categoryId, search, level, priceType, priceFrom, priceTo);
            return await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetFilteredCountAsync(
            int? categoryId, string? search, Level? level,
            string? priceType, decimal? priceFrom, decimal? priceTo)
        {
            var query = ApplyFilters(BaseQuery(), categoryId, search, level, priceType, priceFrom, priceTo);
            return await query.CountAsync();
        }

        public async Task<Course?> GetDetailByIdAsync(int id)
            => await _dbSet
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Sections).ThenInclude(s => s.Lessons)
                .Include(c => c.Reviews)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<List<Course>> GetPopularAsync(int count)
            => await _dbSet
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(count)
                .ToListAsync();

        public async Task<int> GetCountByInstructorAsync(int instructorId)
            => await _dbSet.CountAsync(c => c.InstructorId == instructorId);

        public async Task<List<Course>> SearchAsync(string query, int? categoryId, int take)
        {
            var queryable = _dbSet
                .Include(c => c.Instructor) 
                .Where(c => c.Title.Contains(query) || c.ShortDescription.Contains(query))
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
                queryable = queryable.Where(c => c.CategoryId == categoryId.Value);

            return await queryable
                .OrderBy(c => c.Title)
                .Take(take)
                .ToListAsync();
        }

        private static IQueryable<Course> ApplyFilters(
            IQueryable<Course> query, int? categoryId, string? search, Level? level,
            string? priceType, decimal? priceFrom, decimal? priceTo)
        {
            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(c => c.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Title.Contains(search) || c.ShortDescription.Contains(search));

            if (level.HasValue)
                query = query.Where(c => c.LevelForStudent == level.Value);

            if (priceType == "free")
                query = query.Where(c => c.Price == 0);
            else if (priceType == "paid")
                query = query.Where(c => c.Price > 0);
            else if (priceType == "range")
            {
                if (priceFrom.HasValue) query = query.Where(c => c.Price >= priceFrom.Value);
                if (priceTo.HasValue) query = query.Where(c => c.Price <= priceTo.Value);
            }

            return query;
        }
    }
}
