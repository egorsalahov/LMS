using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Repositories
{
    public class InstructorRepository : Repository<Instructor>, IInstructorRepository
    {
        public InstructorRepository(AppDbContext context) : base(context) { }

        public async Task<Instructor?> GetByEmailAsync(string email)
            => await _dbSet.FirstOrDefaultAsync(i => i.Email == email);

        public async Task<Instructor?> GetByEmailWithCoursesAsync(string email)
            => await _dbSet
                .Include(i => i.Courses).ThenInclude(c => c.Enrollments)
                .Include(i => i.Courses).ThenInclude(c => c.Reviews)
                .FirstOrDefaultAsync(i => i.Email == email);

        public async Task<Instructor?> GetByEmailWithProfileAsync(string email)
            => await _dbSet
                .Include(i => i.Educations)
                .Include(i => i.Experiences)
                .FirstOrDefaultAsync(i => i.Email == email);

        public async Task<int> GetTotalCountAsync()
            => await _dbSet.CountAsync();
    }
}
