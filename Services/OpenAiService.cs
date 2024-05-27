using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using OpenAI_API.Models;

namespace FantasyChas_Backend.Services
{
    public interface IOpenAiService
    {
        Task<ImageResult> GetAiImageAsync(string prompt);
        Task<ChatResult> GetChatGPTResultAsync(List<ChatMessage> messages);
    }
    public class OpenAiService : IOpenAiService
    {
        private readonly OpenAIAPI _api;
        public OpenAiService(OpenAIAPI api)
        {
            _api = api;
        }

        public async Task<ImageResult> GetAiImageAsync(string prompt)
        {
            try
            {
                var result = await _api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest(prompt, 3, ImageSize._256));
                return result;
            }
            catch
            {
                throw new Exception("AI failed creating an image");
            }

        }

        public async Task<ChatResult> GetChatGPTResultAsync(List<ChatMessage> messages)
        {
            try
            {
                ChatResult result = await _api.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.1,
                    Messages = messages.ToArray()
                });

                return result;
            }
            catch
            {
                throw new Exception("AI failed answering the prompt");
            }
        }
    }
}
