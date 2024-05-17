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
using Microsoft.EntityFrameworkCore;
using OpenAI_API.Chat;
using OpenAI_API;
using OpenAI_API.Models;
using Org.BouncyCastle.Asn1.X509;
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

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

        public CharacterController(ILogger<CharacterController> logger, UserManager<IdentityUser> userManager, ICharacterService characterService)
        {
            _logger = logger;
            _userManager = userManager; // Add UserManager to get access to the correct user in the entire DisplayCharacterController.
            _characterService = characterService;
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

        [HttpPost("CreateCharacterForUserWithChatGPT")]
        public async Task<IActionResult> CreateCharacterForUserWithChatGPTAsync([FromBody] JObject jsonObject, OpenAIAPI api)
        {
            // Messages to send
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatMessageRole.System, "Du är en dungeon master för spelet Dungeons and Dragons. Din uppgift är att skapa en karaktär för spelet baserat på reglerna i Dungeons and Dragons. Svara i JSON-format."),
                //new ChatMessage(ChatMessageRole.User, $"Min karaktär: Namn: {character.Name}, HP: {character.HP}, Yrke: {character.Occupation}, Ras: {character.Race}, Level: {character.Level}, Ålder: {character.Age}, Attributes: Styrka: {character.Attributes.Strength}, Smidighet: {character.Attributes.Dexterity}, Intelligens: {character.Attributes.Intelligence}, Vishet: {character.Attributes.Wisdom}, Karisma: {character.Attributes.Charisma}, Constitution: {character.Attributes.Constitution}, Bakgrund: {character.Background}"),
                //new ChatMessage(ChatMessageRole.Assistant, "Du väcks tidigt på morgonen av att sirener ljuder över området. Vad gör du?"),
            };

            // Include chat history
            //foreach (var chatRow in chatHistory)
            //{
            //    messages.Add(new ChatMessage(ChatMessageRole.User, chatRow.Prompt));
            //    messages.Add(new ChatMessage(ChatMessageRole.Assistant, chatRow.Answer));
            //}

            // Add user query (optional, depending on your needs)
            // messages.Add(new ChatMessage(ChatMessageRole.User, query));

            // Get response from AI
            var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.1,
                ResponseFormat = ChatRequest.ResponseFormats.JsonObject,
                Messages = messages.ToArray()
            });

            var character = jsonObject.ToObject<CharacterDto>();

            return new JsonResult(character);
        }
    }
}
