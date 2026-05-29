using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Services;

namespace EgorSalahovSemestrovka22.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<Student> _userManager;
        private readonly ChatService _chatService;

        public ChatController(UserManager<Student> userManager, ChatService chatService)
        {
            _userManager = userManager;
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(string contactId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var messages = await _chatService.GetMessagesAsync(currentUserId, contactId);
            return Json(messages);
        }
    }
}
