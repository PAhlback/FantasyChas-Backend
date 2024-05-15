using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// Hämta karaktärernas ID, från frontend ? Kolla upp hur detta sker. Går det att se någonstans i fetchen?
// Att göra: Kolla Frontends API endpoint, FÖRSÖK! hämta karaktärens id, om inte, försök hänvisa från .. repo? Kolla upp.
// 

namespace FantasyChas_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ActiveStoryController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ActiveStoryController> _logger;

        public ActiveStoryController(ILogger<ActiveStoryController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost("/AddActiveStory")]
        public async Task<IActionResult> AddActiveStoryasync(ActiveStoryDto storyDto, IActiveStoryRepository activeStoryRepo, ICharacterRepository characterRepo)
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                // Retrieve the character by Id
                Character character = await characterRepo.GetCharacterByIdAsync(storyDto.CharacterId);

                // Check if the character exists
                if (character == null)
                {
                    return NotFound("Character not found");
                }

                // Add new ActiveStory
                ActiveStory newActiveStory = new ActiveStory()
                {
                    User = user,
                    Name = storyDto.Name,
                    BasePrompt = storyDto.BasePrompt,
                };

                // Save the ActiveStory in the database
                activeStoryRepo.AddActiveStoryAsync(newActiveStory);

                // Associate the ActiveStory with the Character
                await characterRepo.UpdateCharacterWithActiveStory(storyDto.CharacterId, newActiveStory.Id);

                return Ok("Added story and associated with character!");
            }
            catch
            {
                return BadRequest("Bad request");
            }
        }

        private Task<IdentityUser> GetCurrentUserAsync() => _userManager.GetUserAsync(User);
    }
}