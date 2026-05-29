using EgorSalahovSemestrovka22.Models.Entities.EgorSalahovSemestrovka22.Models.Entities;

namespace Sem.Web.Repositories.Interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<List<Message>> GetConversationAsync(string userId1, string userId2);
    }
}
