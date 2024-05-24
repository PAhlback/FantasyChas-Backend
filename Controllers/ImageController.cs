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
    public class ImageController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ChatController> _logger;
        private static ApplicationDbContext? _context;
        private readonly IImageService _imageService;


        public ImageController(ILogger<ChatController> logger, UserManager<IdentityUser> userManager, IImageService imageService)
        {
            _logger = logger;
            _userManager = userManager;
            _imageService = imageService;
        }

        private Task<IdentityUser> GetCurrentUserAsync() => _userManager.GetUserAsync(User);
        
        [HttpPost("CreateProfileImageWithAI")]
        public async Task<IActionResult> GenerateProfileImageAsync(CharacterDto charDto)
        {
            // takes in character and uses it for generating character's profile picture
            
            try
            {
                IdentityUser user = await GetCurrentUserAsync();
                var image = await _imageService.GenerateImageAsync(user, charDto);

                return Ok(image);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
