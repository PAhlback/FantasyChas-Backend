using OpenAI_API.Chat;

namespace FantasyChas_Backend.Services.ServiceInterfaces
{
    public interface IOpenAiService
    {
        public Task<ChatResult> GetChatGPTResultAsync(List<ChatMessage> messages);
    }
}
