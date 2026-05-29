using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sem.Web.Services;
using EgorSalahovSemestrovka22.Models.Entities;

namespace EgorSalahovSemestrovka22.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<Student> _userManager;
        private readonly ChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(UserManager<Student> userManager, ChatService chatService, ILogger<ChatController> logger)
        {
            _userManager = userManager;
            _chatService = chatService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(string contactId)
        {
            var currentUserId = _userManager.GetUserId(User);
            _logger.LogInformation("Загрузка сообщений от {User} к {Contact}", currentUserId, contactId);
            var messages = await _chatService.GetMessagesAsync(currentUserId, contactId);
            return Json(messages);
        }
    }
}