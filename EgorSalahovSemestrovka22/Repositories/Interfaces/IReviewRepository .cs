using EgorSalahovSemestrovka22.Models.Entities;

namespace Sem.Web.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<Review?> FindByStudentAndCourseAsync(string studentId, int courseId);
        Task<double> GetAverageRatingAsync(int courseId);
        Task<int> GetReviewCountAsync(int courseId);
    }
}
