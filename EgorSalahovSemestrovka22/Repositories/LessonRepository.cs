using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Repositories
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        public LessonRepository(AppDbContext context) : base(context) { }

        public async Task<Lesson?> GetByIdWithSectionAsync(int lessonId)
            => await _dbSet
                .Include(l => l.Section)
                .FirstOrDefaultAsync(l => l.Id == lessonId);
    }
}
