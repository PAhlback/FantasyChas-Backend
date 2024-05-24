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
    }
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadImageAsync(string containerName, string filePath)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            string fileName = Path.GetFileName(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            using FileStream uploadFileStream = File.OpenRead(filePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();

            return blobClient.Uri.ToString();
        }
    }
}
