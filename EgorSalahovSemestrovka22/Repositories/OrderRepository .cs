using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }

        public async Task<List<Order>> GetByStudentAsync(string studentId)
            => await _dbSet
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Course)
                .Where(o => o.StudentId == studentId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

        public async Task<Order?> GetByIdAndStudentAsync(int orderId, string studentId)
            => await _dbSet
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Course)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.StudentId == studentId);

        public async Task<int> GetTotalCountAsync()
            => await _dbSet.CountAsync();

        public async Task<decimal> GetTotalRevenueAsync()
            => await _dbSet.SumAsync(o => o.TotalAmount);

        public async Task<List<object>> GetOrderChartDataAsync()
        {
            var data = await _dbSet
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Revenue = g.Sum(o => o.TotalAmount) })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            return data.Select(o => (object)new
            {
                Label = new DateTime(o.Year, o.Month, 1).ToString("MMM yy"),
                Orders = o.Count,
                Revenue = o.Revenue
            }).ToList();
        }

        public async Task<List<object>> GetRecentOrdersAsync(int count)
            => await _dbSet
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Course)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .Select(o => (object)new
                {
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    o.OrderStatus,
                    CustomerName = o.FirstName + " " + o.LastName,
                    Items = o.OrderItems.Select(oi => oi.Course.Title).ToList()
                })
                .ToListAsync();
    }
}
