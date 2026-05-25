using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Student> _userManager;

        public ChatController(AppDbContext context, UserManager<Student> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(string contactId)
        {
            var currentUserId = _userManager.GetUserId(User);

            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == contactId) ||
                            (m.SenderId == contactId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            var result = new List<object>();
            foreach (var msg in messages)
            {
                string senderName = "Unknown";

                if (msg.SenderId == currentUserId)
                {
                    senderName = "You";
                }
                else
                {
                    var user = await _context.Users.FindAsync(msg.SenderId);
                    var instructor = await _context.Instructors
                        .FirstOrDefaultAsync(i => i.Email == user.Email);

                    if (instructor != null)
                    {
                        senderName = $"{instructor.FirstName} {instructor.LastName}".Trim();
                    }
                    else if (user != null)
                    {
                        senderName = $"{user.FirstName} {user.LastName}".Trim();
                    }
                }

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

            return Json(result);
        }
    }
}
