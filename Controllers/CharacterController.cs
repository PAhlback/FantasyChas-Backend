using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FantasyChas_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CharacterController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(ILogger<CharacterController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;

            // Added UserManager to get access to the correct user in the entire DisplayCharacterController.
            _userManager = userManager;
        }

        [HttpGet("/GetCharacter")]
        public async Task<IActionResult> GetAllCharacters(ICharacterRepository repo)// ta in aktiv user och välj characters för den
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                List<CharacterViewModel> characters = repo.GetCharactersForUser(user.Id);

                return Ok(characters);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/AddCharacter")]
        public async Task<IActionResult> AddCharacter(CharacterDto charDto, ICharacterRepository repo)// här kommer det va Dto, eventuellt UserId separat inparameter
        {
            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                // här kommer vi göra om Dto till model
                Character newCharacter = new Character()
                {
                    Name = charDto.Name,
                    Age = charDto.Age,
                    UserId = user.Id,
                    Gender = charDto.Gender,
                    Level = charDto.Level,
                    HealthPoints = charDto.HealthPoints,
                    Strength = charDto.Strength,
                    Dexterity = charDto.Dexterity,
                    Intelligence = charDto.Intelligence,
                    Wisdom = charDto.Wisdom,
                    Constitution = charDto.Constitution,
                    Charisma = charDto.Charisma,
                    Backstory = charDto.Backstory
                };
                //spara i Db
                repo.AddCharacterToUser(newCharacter);
                
                return Ok("Created character!");
            }
            catch
            {
                return BadRequest("Bad bad");
            }

        }


        // Methods for class
        private Task<IdentityUser> GetCurrentUserAsync() => _userManager.GetUserAsync(User);
    }
}
