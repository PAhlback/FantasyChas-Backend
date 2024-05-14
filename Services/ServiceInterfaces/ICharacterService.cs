using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace FantasyChas_Backend.Services.ServiceInterfaces
{
    public interface ICharacterService
    {
        Task<List<CharacterViewModel>> GetCharactersForUser(string userId);
        Task<Character> CreateCharacterAsync(IdentityUser user, CharacterDto charDto);
    }
}
