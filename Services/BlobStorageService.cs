using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System.Text.RegularExpressions;

namespace FantasyChas_Backend.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadImageFromUrlAsync(string imageUrl);
    }
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        public BlobStorageService()
        {
            _blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("BLOB_CONNECTION_STRING"));
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME"));
        }

        public async Task<string> UploadImageFromUrlAsync(string imageUrl)
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
                    // Extract the file name 
                    var uri = new Uri(imageUrl);
                    string fileName = Path.GetFileName(uri.AbsolutePath);

//                    string fileName = Path.GetFileName(imageUrl);
                    fileName = fileName.Replace('%', '_');
                    string extension = Path.GetExtension(fileName);
                    if (extension == "") // DALLE outputs .png without extension or with long extension that starts with .png
                    {
                        fileName += ".png";
                        extension = Path.GetExtension(fileName);
                    }
                    string baseFileName = Path.GetFileNameWithoutExtension(fileName);

                    // Create Blob client in our container
                    var blobClient = _blobContainerClient.GetBlobClient(fileName);

                    // Ensure the file name is unique
                    int counter = 1;
                    while (await blobClient.ExistsAsync())
                    {
                        fileName = $"{baseFileName}_{counter}{extension}";
                        blobClient = _blobContainerClient.GetBlobClient(fileName);
                        counter++;
                    }

                    // Upload the image in correct format
                    // Attention, DALL E returns unspecified format but file seems to be .png
                    var blobHttpHeader = new BlobHttpHeaders();
                    switch (extension.ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                            blobHttpHeader.ContentType = "image/jpeg";
                            break;
                        case ".png":
                            blobHttpHeader.ContentType = "image/png";
                            break;
                        case ".gif":
                            blobHttpHeader.ContentType = "image/gif";
                            break;
                        default:
                            break;
                    }
                    await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });

                    // Return link to the image in our Blob
                    return blobClient.Uri.ToString();
                }
            }
        }
    }
}
