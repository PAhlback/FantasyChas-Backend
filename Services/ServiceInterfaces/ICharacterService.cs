using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace FantasyChas_Backend.Services.ServiceInterfaces
{
    public interface ICharacterService
    {
        Task<List<CharacterViewModel>> GetCharactersForUser(string userId);
        Task CreateCharacterAsync(IdentityUser user, CharacterDto charDto);
        Task UpdateCharacterAsync(IdentityUser user, CharacterWithIdDto charDto);
        Task DeleteCharacterAsync(string userId, int CharacterId);
        Task<bool> CharacterExistsAsync(int characterId, string userId);
        Task ConnectCharToStoryAsync(int characterId, int storyId, string userId);
    }
}
