using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI_API.Chat;
using OpenAI_API;
using OpenAI_API.Models;
using Org.BouncyCastle.Asn1.X509;
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using FantasyChas_Backend.Models;
using Newtonsoft.Json;

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
        private readonly IOpenAiService _openAiService;

        public CharacterController(ILogger<CharacterController> logger, UserManager<IdentityUser> userManager, ICharacterService characterService, IOpenAiService openAiService)
        {
            _logger = logger;
            _userManager = userManager; // Add UserManager to get access to the correct user in the entire DisplayCharacterController.
            _openAiService = openAiService;
            _characterService = characterService;
        }

        // magda undrar : varför "User" här nedan till höger?
        private async Task<IdentityUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(User);


        [HttpGet("GetCharacters")]
        public async Task<IActionResult> GetUserCharactersAsync()
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                List<CharacterViewModel> characters = await _characterService.GetCharactersForUser(user.Id);

                return Ok(characters);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddCharacter")]
        public async Task<IActionResult> AddCharacterAsync(CharacterDto charDto)
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                await _characterService.CreateCharacterAsync(user, charDto);

                return Ok("Character successfully created and added!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("UpdateCharacter")]
        public async Task<IActionResult> UpdateCharacterAsync(CharacterWithIdDto charDto)
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                await _characterService.UpdateCharacterAsync(user, charDto);

                return Ok("Character successfully updated!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteCharacter")]
        public async Task<IActionResult> DeleteCharacterAsync(int characterId)
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();
                string userId = user.Id;

                await _characterService.DeleteCharacterAsync(userId, characterId);

                return Ok($"Character with ID {characterId} successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateCharacterWithAi")]
        public async Task<IActionResult> CreateCharacterWithAiAsync(NewCharacterViewModel character)
        {
            try
            {
                NewCharacterViewModel newCharacter = await _characterService.CreateCharacterWithAiAsync(character);

                return Ok(newCharacter);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}
