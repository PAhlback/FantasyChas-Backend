using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FantasyChas_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ChatController> _logger;
        private static ApplicationDbContext? _context;
        private readonly IChatService _chatService;


        public ChatController(ILogger<ChatController> logger, UserManager<IdentityUser> userManager, IChatService chatService)
        {
            _logger = logger;
            _userManager = userManager;
            _chatService = chatService;
        }

        private Task<IdentityUser> GetCurrentUserAsync() => _userManager.GetUserAsync(User);

        // posta user input;
        // get svar från AI 
        // get senaste XX meddelanden i storyn - alternativt föregående chat

        [HttpPost("Message")]
        public async Task<IActionResult> SendChatMessageAsync(StoryChatPromptDto chatPromptObject)
        {
            try
            {
                var jsonObject = await _chatService.SendToChatServiceAsync(chatPromptObject);

                return Ok(jsonObject);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}

