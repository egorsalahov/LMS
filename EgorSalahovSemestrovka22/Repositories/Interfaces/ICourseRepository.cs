using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Enums;

namespace Sem.Web.Repositories.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<List<Course>> GetFilteredAsync(
            int? categoryId, string? search, Level? level,
            string? priceType, decimal? priceFrom, decimal? priceTo,
            int page, int pageSize);

        Task<int> GetFilteredCountAsync(
            int? categoryId, string? search, Level? level,
            string? priceType, decimal? priceFrom, decimal? priceTo);

        Task<Course?> GetDetailByIdAsync(int id);
        Task<List<Course>> GetPopularAsync(int count);
        Task<int> GetCountByInstructorAsync(int instructorId);
        Task<List<Course>> SearchAsync(string query, int? categoryId, int take);
    }
}
