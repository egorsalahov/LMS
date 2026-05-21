using EgorSalahovSemestrovka22.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin – Analysis
        public async Task<IActionResult> Index()
        {
            // Total Users = Students (AspNetUsers) + Instructors
            var studentCount = await _context.Users.CountAsync();
            var instructorCount = await _context.Instructors.CountAsync();
            ViewBag.TotalUsers = studentCount + instructorCount;

            // New Users – последние 7 студентов
            var newUsers = await _context.Users
                .OrderByDescending(u => u.RegistrationDate)
                .Take(7)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.UserName,
                    u.AvatarPath,
                    u.RegistrationDate
                })
                .ToListAsync();
            ViewBag.NewUsers = newUsers;

            // Recent Orders – последние 5 заказов
            var recentOrders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
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
            ViewBag.RecentOrders = recentOrders;

            // Данные для графика регистраций (студенты + инструкторы по месяцам)
            var studentRegistrations = await _context.Users
                .GroupBy(u => new { u.RegistrationDate.Year, u.RegistrationDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var instructorRegistrations = await _context.Instructors
                .GroupBy(i => new { i.RegistrationDate.Year, i.RegistrationDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var allDates = studentRegistrations.Select(s => new DateTime(s.Year, s.Month, 1))
                .Union(instructorRegistrations.Select(i => new DateTime(i.Year, i.Month, 1)))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            var allMonths = new List<object>();
            foreach (var date in allDates)
            {
                var monthlyStudents = studentRegistrations
                    .Where(s => s.Year == date.Year && s.Month == date.Month)
                    .Sum(s => s.Count);
                var monthlyInstructors = instructorRegistrations
                    .Where(i => i.Year == date.Year && i.Month == date.Month)
                    .Sum(i => i.Count);

                allMonths.Add(new
                {
                    Label = date.ToString("MMM yy"),
                    Students = monthlyStudents,
                    Instructors = monthlyInstructors,
                    Total = monthlyStudents + monthlyInstructors
                });
            }
            ViewBag.RegistrationData = allMonths;

            return View();
        }

        // GET: /Admin/ECommerce – eCommerce Dashboard
        public async Task<IActionResult> ECommerce()
        {
            var totalOrders = await _context.Orders.CountAsync();
            ViewBag.TotalOrders = totalOrders;

            var totalSales = await _context.Orders.SumAsync(o => o.TotalAmount);
            ViewBag.TotalSales = totalSales;

            var popularCourses = await _context.OrderItems
                .Include(oi => oi.Course)
                .GroupBy(oi => oi.Course)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new
                {
                    Course = g.Key,
                    OrdersCount = g.Count(),
                    TotalRevenue = g.Sum(oi => oi.PriceAtPurchase)
                })
                .ToListAsync();
            ViewBag.PopularCourses = popularCourses;

            var topInstructors = await _context.OrderItems
                .Include(oi => oi.Course)
                    .ThenInclude(c => c.Instructor)
                .Where(oi => oi.Course.Instructor != null)
                .GroupBy(oi => oi.Course.Instructor)
                .OrderByDescending(g => g.Sum(oi => oi.PriceAtPurchase))
                .Take(5)
                .Select(g => new
                {
                    Instructor = g.Key,
                    TotalSales = g.Sum(oi => oi.PriceAtPurchase),
                    OrdersCount = g.Count()
                })
                .ToListAsync();
            ViewBag.TopInstructors = topInstructors;

            // Данные для графика заказов по месяцам
            var orderData = await _context.Orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Revenue = g.Sum(o => o.TotalAmount) })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var orderChartData = orderData.Select(o => new
            {
                Label = new DateTime(o.Year, o.Month, 1).ToString("MMM yy"),
                Orders = o.Count,
                Revenue = o.Revenue
            }).ToList();

            ViewBag.OrderChartData = orderChartData;

            return View();
        }
    }
}