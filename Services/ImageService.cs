using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Images;

namespace FantasyChas_Backend.Services
{
    public interface IImageService
    {

        Task<ImageResult> GenerateImageAsync(IdentityUser user, CharacterDto charDto);

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

        public async Task<ImageResult> GenerateImageAsync(IdentityUser user, CharacterDto charDto)
        {
            try
            {
                // skapar beskrivning av föfrågan
                string? serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(charDto);
                string prompt = $"Skapa en profilbild till för en karaktär i Dungeons and Masters. Storlek 200px*300px. Karaktär: {serializedObject}";

                // skicka frågan till openAIService 
                var response = await _openAiService.GetAiImageAsync(prompt);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create character", ex);
            }

        }
    }
}
