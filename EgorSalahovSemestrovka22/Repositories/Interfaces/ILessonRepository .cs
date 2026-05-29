using EgorSalahovSemestrovka22.Models.Entities;

namespace Sem.Web.Repositories.Interfaces
{
    public interface ILessonRepository : IRepository<Lesson>
    {
        Task<Lesson?> GetByIdWithSectionAsync(int lessonId);
    }
}
