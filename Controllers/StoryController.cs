using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
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
    public class StoryController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<StoryController> _logger;
        private Task<IdentityUser> GetCurrentUserAsync() => _userManager.GetUserAsync(User);

        public StoryController(ILogger<StoryController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet("/GetSelectedStory")]
        public async Task<IActionResult> GetSelectedStory()//repo for stories
        {

        }

    }
}


