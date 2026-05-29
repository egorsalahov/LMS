using EgorSalahovSemestrovka22.Models.Entities;

namespace Sem.Web.Repositories.Interfaces
{
    public interface IWishlistRepository : IRepository<Wishlist>
    {
        Task<List<Wishlist>> GetByStudentAsync(string studentId);
        Task<List<int>> GetCourseIdsAsync(string studentId);
        Task<Wishlist?> FindByStudentAndCourseAsync(string studentId, int courseId);
    }
}
