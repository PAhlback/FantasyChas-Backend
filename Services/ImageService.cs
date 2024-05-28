using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FantasyChas_Backend.Services
{
    public interface IImageService
    {

        Task<ImageViewModel> GenerateImageAsync(IdentityUser user, CharacterDto charDto);

    }
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IOpenAiService _openAiService;

        public ImageService(IImageRepository imageRepository, IOpenAiService openAiService)
        {
            _imageRepository = imageRepository;
            _openAiService = openAiService;
        }

        public async Task<ImageViewModel> GenerateImageAsync(IdentityUser user, CharacterDto charDto)
        {
            try
            {
                string? serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(charDto);
                string prompt = $"Skapa en profilbild för en karaktär i Dungeons and Masters. Gärna drömlik. Karaktär: {serializedObject}";

                var response = await _openAiService.GetAiImageAsync(prompt);

                List<string> urls = new List<string>();
                foreach (var dataLine in response.Data)
                {
                    urls.Add(dataLine.Url);
                }

                ImageViewModel result = new ImageViewModel()
                {
                    Urls = urls,
                    ExpiryTime = response.Created?.AddHours(1).ToLocalTime(),
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create character image", ex);
            }

        }
    }

}
