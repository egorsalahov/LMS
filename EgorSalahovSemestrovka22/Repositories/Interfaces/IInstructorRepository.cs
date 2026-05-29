using EgorSalahovSemestrovka22.Models.Entities.Instructors;

namespace Sem.Web.Repositories.Interfaces
{
    public interface IInstructorRepository : IRepository<Instructor>
    {
        Task<Instructor?> GetByEmailAsync(string email);
        Task<Instructor?> GetByEmailWithCoursesAsync(string email);
        Task<Instructor?> GetByEmailWithProfileAsync(string email);
        Task<int> GetTotalCountAsync();
    }
}
