using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities.EgorSalahovSemestrovka22.Models.Entities;
using Sem.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Sem.Web.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context) { }

        public async Task<List<Message>> GetConversationAsync(string userId1, string userId2)
            => await _dbSet
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
    }
}
