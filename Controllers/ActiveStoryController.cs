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
    public class ActiveStoryController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ActiveStoryController> _logger;
        private readonly ICharacterService _characterService;
        private readonly IActiveStoryService _activeStoryService;

        public ActiveStoryController(ILogger<ActiveStoryController> logger, UserManager<IdentityUser> userManager, ICharacterService characterService, IActiveStoryService activeStoryService)
        {
            _logger = logger;
            _userManager = userManager;
            _characterService = characterService;
            _activeStoryService = activeStoryService;
        }

        private async Task<IdentityUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(User);

        [HttpPost("AddStory")]
        public async Task<IActionResult> AddStoryAsync(ActiveStoryDto storyDto)
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                bool characterExists = await _characterService.CharacterExistsAsync(storyDto.CharacterId, user.Id);
                if (!characterExists)
                {
                    return NotFound("Character not found");
                }

                int storyId = await _activeStoryService.CreateStoryAsync(user, storyDto);

                await _characterService.ConnectCharToStoryAsync(storyDto.CharacterId, storyId, user.Id);

                return Ok("Added story and associated with character!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
