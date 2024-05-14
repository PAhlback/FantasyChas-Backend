using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Repositories;
using FantasyChas_Backend.Services;
using FantasyChas_Backend.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FantasyChas_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CharacterController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CharacterController> _logger;
        private readonly ICharacterService _characterService;


        public CharacterController(ILogger<CharacterController> logger, UserManager<IdentityUser> userManager, ICharacterService characterServices)
        {
            _logger = logger;
            _userManager = userManager; // Add UserManager to get access to the correct user in the entire DisplayCharacterController.
            _characterService = characterServices;
        }

        // magda undrar : varför "User" här nedan till höger?
        private async Task<IdentityUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(User);


        [HttpGet("GetCharacters")]
        public async Task<IActionResult> GetUserCharactersAsync(ICharacterService characterService)
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                List<CharacterViewModel> characters = await characterService.GetCharactersForUser(user.Id);

                return Ok(characters);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateCharacter")]
        public async Task<IActionResult> CreateCharacterAsync(ICharacterService characterService, CharacterDto charDto)
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                Character character = await _characterService.CreateCharacterAsync(user, charDto);
                
                return Ok(character);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("UpdateCharacter")]
        public async Task<IActionResult> UpdateCharacterAsync(ICharacterService characterService, CharacterWithIdDto charDto)
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                Character character = await _characterService.UpdateCharacterAsync(user, charDto);

                return Ok("Character successfully updated!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteCharacter")]
        public async Task<IActionResult> DeleteCharacterAsync(ICharacterService characterService, int characterId)
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();
                string userId = user.Id;

                await characterService.DeleteCharacterAsync(userId, characterId);

                return Ok($"Character with ID {characterId} successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
