using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;

namespace Sem.Web.Areas.Admin.Repositories.Interfaces
{
    public interface IAdminRepository
    {

        Task<int> GetStudentCountAsync();
        Task<int> GetInstructorCountAsync();
        Task<List<object>> GetNewUsersAsync(int count);
        Task<List<object>> GetRecentOrdersAsync(int count);
        Task<List<object>> GetStudentRegistrationsByMonthAsync();
        Task<List<object>> GetInstructorRegistrationsByMonthAsync();


        Task<int> GetTotalOrdersCountAsync();
        Task<decimal> GetTotalSalesAsync();
        Task<List<object>> GetPopularCoursesAsync(int count);
        Task<List<object>> GetTopInstructorsAsync(int count);
        Task<List<object>> GetOrderChartDataAsync();


        Task<List<Instructor>> GetAllInstructorsWithDetailsAsync();
        Task<List<Student>> GetAllStudentsWithDetailsAsync();


        Task<List<Course>> GetAllCoursesWithDetailsAsync();
        Task<List<object>> GetCustomersAsync();
        Task<List<object>> GetAllOrdersAsync();
    }
}
