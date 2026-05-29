using EgorSalahovSemestrovka22.Models.Entities;

namespace Sem.Web.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetAllWithCoursesAsync();
        Task<List<object>> GetCategoryListAsync();
        Task<Category?> GetWithCoursesAsync(int id);
    }
}
