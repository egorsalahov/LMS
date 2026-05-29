using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Repositories
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context) { }

        public async Task<List<Enrollment>> GetStudentEnrollmentsAsync(string studentId)
            => await _dbSet
                .Include(e => e.Course).ThenInclude(c => c.Category)
                .Include(e => e.Course).ThenInclude(c => c.Instructor)
                .Include(e => e.Course).ThenInclude(c => c.Reviews)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

        public async Task<Enrollment?> GetByIdWithCourseAsync(string userId, int enrollmentId)
            => await _dbSet
                .Include(e => e.Course).ThenInclude(c => c.Sections).ThenInclude(s => s.Lessons)
                .Include(e => e.Course).ThenInclude(c => c.Instructor)
                .Include(e => e.Course).ThenInclude(c => c.Category)
                .Include(e => e.Course).ThenInclude(c => c.Reviews)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.StudentId == userId);

        public async Task<Enrollment?> GetByIdForLessonAsync(string userId, int enrollmentId)
            => await _dbSet
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.StudentId == userId);

        public async Task<List<Instructor>> GetStudentInstructorsAsync(string studentId)
            => await _dbSet
                .Where(e => e.StudentId == studentId)
                .Select(e => e.Course.Instructor)
                .Where(i => i != null)
                .Distinct()
                .ToListAsync() as List<Instructor> ?? new List<Instructor>();

        public async Task<List<Student>> GetInstructorStudentsAsync(int instructorId)
            => await _dbSet
                .Where(e => e.Course.InstructorId == instructorId)
                .Select(e => e.Student)
                .Distinct()
                .ToListAsync();
    }
}
