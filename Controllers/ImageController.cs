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
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromBody] ImageRequest request)
        {
            try
            {
                //string localPath = await DownloadAndSaveImageAsync(request.ImageUrl, "path/to/save/images");
                string imageUrl = "https://fastly.picsum.photos/id/237/200/300.jpg?hmac=TmmQSbShHz9CdQm0NkEjx1Dyh_Y984R9LpNrpvH2D_U";
                //string connectionString = "https://fantasychasblobtest.blob.core.windows.net/fantasty-test";
                //string connectionString = "";
                var blobStorageService = new BlobStorageService();
                string imageAzureUrl = await blobStorageService.UploadImageFromUrlAsync("fantasty-test", imageUrl);

                return Ok(new { Url = imageAzureUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
    public class ImageRequest
    {
        public string ImageUrl { get; set; }
    }
}
