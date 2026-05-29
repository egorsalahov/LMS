using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;

namespace Sem.Web.Services
{
    public class ChatService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly IRepository<Student> _userRepo;

        public ChatService(
            IMessageRepository messageRepo,
            IInstructorRepository instructorRepo,
            IRepository<Student> userRepo)
        {
            _messageRepo = messageRepo;
            _instructorRepo = instructorRepo;
            _userRepo = userRepo;
        }

        public async Task<List<object>> GetMessagesAsync(string currentUserId, string contactId)
        {
            var messages = await _messageRepo.GetConversationAsync(currentUserId, contactId);

            var result = new List<object>();
            foreach (var msg in messages)
            {
                var senderName = await GetSenderNameAsync(msg.SenderId, currentUserId);
                result.Add(new
                {
                    msg.Id,
                    msg.SenderId,
                    msg.ReceiverId,
                    msg.Content,
                    msg.Timestamp,
                    SenderName = senderName
                });
            }

            return result;
        }

        private async Task<string> GetSenderNameAsync(string senderId, string currentUserId)
        {
            if (senderId == currentUserId)
                return "You";

            var user = await _userRepo.GetByIdAsync(senderId);
            if (user == null)
                return "Unknown";

            var instructor = await _instructorRepo.GetByEmailAsync(user.Email);
            if (instructor != null)
                return $"{instructor.FirstName} {instructor.LastName}".Trim();

            return $"{user.FirstName} {user.LastName}".Trim();
        }
    }
}
