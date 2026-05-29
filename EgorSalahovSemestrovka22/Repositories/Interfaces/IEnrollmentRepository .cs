using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;

namespace Sem.Web.Repositories.Interfaces
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<List<Enrollment>> GetStudentEnrollmentsAsync(string studentId);
        Task<Enrollment?> GetByIdWithCourseAsync(string userId, int enrollmentId);
        Task<Enrollment?> GetByIdForLessonAsync(string userId, int enrollmentId);
        Task<List<Instructor>> GetStudentInstructorsAsync(string studentId);
        Task<List<Student>> GetInstructorStudentsAsync(int instructorId);
    }
}
