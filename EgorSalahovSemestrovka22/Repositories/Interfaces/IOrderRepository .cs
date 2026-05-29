using EgorSalahovSemestrovka22.Models.Entities.Orders;

namespace Sem.Web.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetByStudentAsync(string studentId);
        Task<Order?> GetByIdAndStudentAsync(int orderId, string studentId);
        Task<int> GetTotalCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<List<object>> GetOrderChartDataAsync();
        Task<List<object>> GetRecentOrdersAsync(int count);
    }
}
