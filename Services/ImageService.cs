using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Images;
using Azure.Storage.Blobs;

namespace FantasyChas_Backend.Services
{
    public interface IImageService
    {

        Task<ImageViewModel> GenerateImageAsync(IdentityUser user, CharacterDto charDto);
        Task<string> SaveImageToBlobAsync(int characterId, string imageUrl);

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

        public Task<string> SaveImageToBlobAsync(int characterId, string imageUrl)
        {
            throw new NotImplementedException();
        }
        public async Task<string> DownloadAndSaveImageAsync(string imageUrl, string savePath)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(imageUrl);
                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    string fileName = Path.GetFileName(imageUrl);
                    string fullPath = Path.Combine(savePath, fileName);

                    await File.WriteAllBytesAsync(fullPath, imageBytes);

                    return fullPath; // Return the full path of the saved image
                }
                else
                {
                    throw new Exception("Failed to download image.");
                }
            }
        }
    }
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService()
        {
            _blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("CONNECTION_STRING_BLOB"));

        }

        public async Task<string> UploadImageFromUrlAsync(string containerName, string imageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(imageUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to download image from URL.");
                }

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    // Extrahera filnamnet från URL:en
                    string fileName = Path.GetFileName(imageUrl);
                    
                        fileName += ".png";
                    

                    // Hämta blob-behållare
                    var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                    // Skapa en BlobClient för att ladda upp bilden
                    var blobClient = containerClient.GetBlobClient(fileName);

                    // Ladda upp bilden till blob-lagringen
                    await blobClient.UploadAsync(stream, true);

                    // Returnera URL:en till den uppladdade bilden
                    return blobClient.Uri.ToString();
                }
            }
        }
    }
}
