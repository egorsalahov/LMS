using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;

namespace Sem.Web.Areas.Admin.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        // Аналитика
        Task<int> GetStudentCountAsync();
        Task<int> GetInstructorCountAsync();
        Task<List<object>> GetNewUsersAsync(int count);
        Task<List<object>> GetRecentOrdersAsync(int count);
        Task<List<object>> GetStudentRegistrationsByMonthAsync();
        Task<List<object>> GetInstructorRegistrationsByMonthAsync();

        // ECommerce
        Task<int> GetTotalOrdersCountAsync();
        Task<decimal> GetTotalSalesAsync();
        Task<List<object>> GetPopularCoursesAsync(int count);
        Task<List<object>> GetTopInstructorsAsync(int count);
        Task<List<object>> GetOrderChartDataAsync();

        // Data tables
        Task<List<Instructor>> GetAllInstructorsWithDetailsAsync();
        Task<List<Student>> GetAllStudentsWithDetailsAsync();

        // ECommerce management
        Task<List<Course>> GetAllCoursesWithDetailsAsync();
        Task<List<object>> GetCustomersAsync();
        Task<List<object>> GetAllOrdersAsync();
    }
}
