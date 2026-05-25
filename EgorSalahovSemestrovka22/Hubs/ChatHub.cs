using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities.EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string receiverId, string content)
        {
            var senderId = Context.UserIdentifier;

            bool canChat = await CanUserChat(senderId, receiverId);
            if (!canChat)
            {
                await Clients.Caller.SendAsync("Error", "You are not allowed to message this user.");
                return;
            }

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Получаем имя отправителя
            var user = await _context.Users.FindAsync(senderId);
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Email == user.Email);
            var senderName = instructor != null
                ? $"{instructor.FirstName} {instructor.LastName}".Trim()
                : user?.FirstName ?? "Unknown";

            // Отправляем сообщение получателю
            await Clients.User(receiverId).SendAsync("ReceiveMessage", new
            {
                message.Id,
                message.SenderId,
                message.ReceiverId,
                message.Content,
                message.Timestamp,
                SenderName = senderName
            });

            // Отправляем подтверждение отправителю
            await Clients.Caller.SendAsync("MessageSent", new
            {
                message.Id,
                message.SenderId,
                message.ReceiverId,
                message.Content,
                message.Timestamp
            });
        }

        private async Task<bool> CanUserChat(string userId, string targetUserId)
        {
            var user = await _context.Users.Include(u => u.Enrollments)
                                          .ThenInclude(e => e.Course)
                                          .ThenInclude(c => c.Instructor)
                                          .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            // Получаем целевого пользователя один раз
            var targetUser = await _context.Users.FindAsync(targetUserId);
            if (targetUser == null) return false;

            // Проверяем, является ли пользователь студентом, который купил курс у этого инструктора
            bool isStudentOfInstructor = await _context.Enrollments
                .AnyAsync(e => e.StudentId == userId &&
                               e.Course.Instructor != null &&
                               e.Course.Instructor.Email == targetUser.Email);

            // Проверяем, является ли пользователь инструктором, у которого учится этот студент
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Email == user.Email);
            if (instructor != null)
            {
                bool hasStudent = await _context.Enrollments
                    .AnyAsync(e => e.StudentId == targetUserId &&
                                   e.Course.InstructorId == instructor.Id);
                if (hasStudent) return true;
            }

            return isStudentOfInstructor;
        }
    }
}
