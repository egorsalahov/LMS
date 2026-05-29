using EgorSalahovSemestrovka22.Models.Entities.Orders;

namespace Sem.Web.Repositories.Interfaces
{
    public interface ICartRepository : IRepository<CartItem>
    {
        Task<List<CartItem>> GetByStudentAsync(string studentId);
        Task<int> GetCountByStudentAsync(string studentId);
        Task<CartItem?> GetByIdAndStudentAsync(int id, string studentId);
        Task<List<CartItem>> GetAllByStudentAsync(string studentId);
    }
}
