using FantasyChas_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FantasyChas_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DisplayCharacterController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public Character char1 = new Character()
        {
            Name = "Thor",
            Age = 666,
            /*UserId = 1,*/
            Gender = "En herre",
            Level = 1,
            HealthPoints = 10,
            Strength = 15,
            Dexterity = 14,
            Intelligence = 13,
            Wisdom = 12,
            Constitution = 10,
            Charisma = 8,
            Backstory = "Jag kommer från djupa norrländska skogar"
        };

        private readonly ILogger<DisplayCharacterController> _logger;
        //private readonly IdentityUser _user;
        public DisplayCharacterController(ILogger<DisplayCharacterController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            // Added UserManager to get access to the correct user in the entire DisplayCharacterController.
            _userManager = userManager;
        }

        [HttpGet("/GetCharacter")]
        public async Task<IActionResult> GetAllCharacters()// ta in aktiv user och välj characters för den
        {
            List<Character> characterList = new List<Character>();

            try
            {
                IdentityUser user = await GetCurrentUserAsync();

                characterList.Add(char1);
                Character char2 = new Character() { UserId = user.Id };
                characterList.Add(char2);
                return Ok(characterList);
            }
            catch
            {
                return BadRequest("Bad bad");
            }

        }

        private Task<IdentityUser> GetCurrentUserAsync() => _userManager.GetUserAsync(User);
    }

    //[Route("[controller]")]
    public class AddCharacterController : ControllerBase
    {


        private readonly ILogger<AddCharacterController> _logger;

        public AddCharacterController(ILogger<AddCharacterController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/AddCharacter")]
        public IActionResult AddCharacter(Character charDto)// här kommer det va Dto, eventuellt UserId separat inparameter
        {
            try
            {
                // här kommer vi göra om Dto till model
                Character char1 = new Character()
                {
                    Name = charDto.Name,
                    Age = charDto.Age,
                    /*UserId =user från inparameter,*/
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
                //spara i Db också
                return Ok(char1);
            }
            catch
            {
                return BadRequest("Bad bad");
            }

        }
    }
}
