using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using Sem.Web.Areas.Admin.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Sem.Web.Areas.Admin.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetStudentCountAsync()
            => await _context.Users.CountAsync();

        public async Task<int> GetInstructorCountAsync()
            => await _context.Instructors.CountAsync();

        public async Task<List<object>> GetNewUsersAsync(int count)
        {
            var users = await _context.Users
                .OrderByDescending(u => u.RegistrationDate)
                .Take(count)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.UserName,
                    u.AvatarPath,
                    u.RegistrationDate
                })
                .ToListAsync();
            return users.Cast<object>().ToList();
        }

        public async Task<List<object>> GetRecentOrdersAsync(int count)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    o.OrderStatus,
                    CustomerName = o.FirstName + " " + o.LastName,
                    Items = o.OrderItems.Select(oi => oi.Course.Title).ToList()
                })
                .ToListAsync();
            return orders.Cast<object>().ToList();
        }

        public async Task<List<object>> GetStudentRegistrationsByMonthAsync()
        {
            var data = await _context.Users
                .GroupBy(u => new { u.RegistrationDate.Year, u.RegistrationDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();
            return data.Cast<object>().ToList();
        }

        public async Task<List<object>> GetInstructorRegistrationsByMonthAsync()
        {
            var data = await _context.Instructors
                .GroupBy(i => new { i.RegistrationDate.Year, i.RegistrationDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();
            return data.Cast<object>().ToList();
        }

        public async Task<int> GetTotalOrdersCountAsync()
            => await _context.Orders.CountAsync();

        public async Task<decimal> GetTotalSalesAsync()
            => await _context.Orders.SumAsync(o => o.TotalAmount);

        public async Task<List<object>> GetPopularCoursesAsync(int count)
        {
            var courses = await _context.OrderItems
                .Include(oi => oi.Course)
                .GroupBy(oi => oi.Course)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => new
                {
                    Course = g.Key,
                    OrdersCount = g.Count(),
                    TotalRevenue = g.Sum(oi => oi.PriceAtPurchase)
                })
                .ToListAsync();
            return courses.Cast<object>().ToList();
        }

        public async Task<List<object>> GetTopInstructorsAsync(int count)
        {
            var instructors = await _context.OrderItems
                .Include(oi => oi.Course)
                    .ThenInclude(c => c.Instructor)
                .Where(oi => oi.Course.Instructor != null)
                .GroupBy(oi => oi.Course.Instructor)
                .OrderByDescending(g => g.Sum(oi => oi.PriceAtPurchase))
                .Take(count)
                .Select(g => new
                {
                    Instructor = g.Key,
                    TotalSales = g.Sum(oi => oi.PriceAtPurchase),
                    OrdersCount = g.Count()
                })
                .ToListAsync();
            return instructors.Cast<object>().ToList();
        }

        public async Task<List<object>> GetOrderChartDataAsync()
        {
            var data = await _context.Orders
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

        public async Task<List<Instructor>> GetAllInstructorsWithDetailsAsync()
            => await _context.Instructors
                .Include(i => i.Courses)
                .Include(i => i.Educations)
                .Include(i => i.Experiences)
                .OrderBy(i => i.Id)
                .ToListAsync();

        public async Task<List<Student>> GetAllStudentsWithDetailsAsync()
            => await _context.Users
                .Include(s => s.Enrollments)
                .Include(s => s.Orders)
                .OrderBy(s => s.Id)
                .ToListAsync();

        public async Task<List<Course>> GetAllCoursesWithDetailsAsync()
            => await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .OrderBy(c => c.Id)
                .ToListAsync();

        public async Task<List<object>> GetCustomersAsync()
        {
            var customers = await _context.Instructors
                .Include(i => i.Courses)
                .OrderBy(i => i.FirstName)
                .Select(i => new
                {
                    i.Id,
                    FullName = i.FirstName + " " + i.LastName,
                    i.Email,
                    CoursesCount = i.Courses.Count,
                    TotalEarnings = i.TotalEarnings,
                    i.RegistrationDate
                })
                .ToListAsync();
            return customers.Cast<object>().ToList();
        }

        public async Task<List<object>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Student)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    CustomerName = o.Student != null ? o.Student.FirstName + " " + o.Student.LastName : "Unknown",
                    o.OrderDate,
                    o.OrderStatus
                })
                .ToListAsync();
            return orders.Cast<object>().ToList();
        }
    }
}
