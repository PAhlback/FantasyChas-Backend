using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;

namespace FantasyChas_Backend.Services.ServiceInterfaces
{
    public interface IChatService
    {
        public Task<StoryChatResponseViewModel> SendToChatServiceAsync(StoryChatPromptDto chatPromptObject);
    }
}
