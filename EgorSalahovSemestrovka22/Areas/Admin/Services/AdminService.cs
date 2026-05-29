using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using Sem.Web.Areas.Admin.Repositories.Interfaces;

namespace Sem.Web.Areas.Admin.Services
{
    public class AdminService
    {
        private readonly IAdminRepository _adminRepo;

        public AdminService(IAdminRepository adminRepo)
        {
            _adminRepo = adminRepo;
        }

        public async Task<AdminDashboardData> GetDashboardDataAsync()
        {
            var studentCount = await _adminRepo.GetStudentCountAsync();
            var instructorCount = await _adminRepo.GetInstructorCountAsync();

            var newUsers = await _adminRepo.GetNewUsersAsync(7);
            var recentOrders = await _adminRepo.GetRecentOrdersAsync(5);

            var studentRegs = await _adminRepo.GetStudentRegistrationsByMonthAsync();
            var instructorRegs = await _adminRepo.GetInstructorRegistrationsByMonthAsync();

            // Преобразуем dynamic в словари для удобства
            var studentDict = studentRegs
                .Select(s => new
                {
                    Year = (int)((dynamic)s).Year,
                    Month = (int)((dynamic)s).Month,
                    Count = (int)((dynamic)s).Count
                })
                .ToList();

            var instructorDict = instructorRegs
                .Select(i => new
                {
                    Year = (int)((dynamic)i).Year,
                    Month = (int)((dynamic)i).Month,
                    Count = (int)((dynamic)i).Count
                })
                .ToList();

            // Собираем все уникальные даты
            var allDates = studentDict
                .Select(s => new DateTime(s.Year, s.Month, 1))
                .Union(instructorDict.Select(i => new DateTime(i.Year, i.Month, 1)))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            // Формируем данные для графика
            var allMonths = new List<object>();
            foreach (var date in allDates)
            {
                var monthlyStudents = studentDict
                    .Where(s => s.Year == date.Year && s.Month == date.Month)
                    .Sum(s => s.Count);

                var monthlyInstructors = instructorDict
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

            return new AdminDashboardData
            {
                TotalUsers = studentCount + instructorCount,
                NewUsers = newUsers,
                RecentOrders = recentOrders,
                RegistrationChartData = allMonths
            };
        }

        public async Task<ECommerceDashboardData> GetECommerceDataAsync()
        {
            var totalOrders = await _adminRepo.GetTotalOrdersCountAsync();
            var totalSales = await _adminRepo.GetTotalSalesAsync();
            var popularCourses = await _adminRepo.GetPopularCoursesAsync(5);
            var topInstructors = await _adminRepo.GetTopInstructorsAsync(5);
            var orderChartData = await _adminRepo.GetOrderChartDataAsync();

            return new ECommerceDashboardData
            {
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                PopularCourses = popularCourses,
                TopInstructors = topInstructors,
                OrderChartData = orderChartData
            };
        }

        public async Task<List<Instructor>> GetAllInstructorsAsync()
            => await _adminRepo.GetAllInstructorsWithDetailsAsync();

        public async Task<List<Student>> GetAllStudentsAsync()
            => await _adminRepo.GetAllStudentsWithDetailsAsync();

        public async Task<List<Course>> GetAllCoursesAsync()
            => await _adminRepo.GetAllCoursesWithDetailsAsync();

        public async Task<List<object>> GetCustomersAsync()
            => await _adminRepo.GetCustomersAsync();

        public async Task<List<object>> GetAllOrdersAsync()
            => await _adminRepo.GetAllOrdersAsync();
    }

    // Простые DTO для передачи данных
    public class AdminDashboardData
    {
        public int TotalUsers { get; set; }
        public List<object> NewUsers { get; set; }
        public List<object> RecentOrders { get; set; }
        public List<object> RegistrationChartData { get; set; }
    }

    public class ECommerceDashboardData
    {
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public List<object> PopularCourses { get; set; }
        public List<object> TopInstructors { get; set; }
        public List<object> OrderChartData { get; set; }
    }
}
