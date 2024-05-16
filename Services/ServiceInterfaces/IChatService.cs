using FantasyChas_Backend.Models.DTOs;

namespace FantasyChas_Backend.Services.ServiceInterfaces
{
    public interface IChatService
    {
        Task CreateCharacterForUserWithChatGPT(CharacterDto charDto);
    }
}
