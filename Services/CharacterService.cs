using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Repositories;
using FantasyChas_Backend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using FantasyChas_Backend.Services.ServiceInterfaces;

namespace FantasyChas_Backend.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;

        public CharacterService(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        public async Task<List<CharacterViewModel>> GetCharactersForUser(string userId)
        {
            try
            {
                var characters = await _characterRepository.GetCharacters();

                if (characters.Count() == 0)
                {
                    throw new Exception("No characters found!");
                }

                List<CharacterViewModel> userCharacters = characters
                    .Where(u => u.User.Id == userId)
                    .Select(c => new CharacterViewModel()
                    {
                        Name = c.Name,
                        Age = c.Age,
                        Gender = c.Gender,
                        Level = c.Level,
                        HealthPoints = c.HealthPoints,
                        Strength = c.Strength,
                        Dexterity = c.Dexterity,
                        Intelligence = c.Intelligence,
                        Wisdom = c.Wisdom,
                        Constitution = c.Constitution,
                        Charisma = c.Charisma,
                        Backstory = c.Backstory,
                        Profession = c.Profession,
                        Species = c.Species
                    })
                    .ToList();

                return userCharacters;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get characters", ex);
            }
        }

        public async Task<Character> CreateCharacterAsync(string userId, CharacterDto charDto)
        {
            try
            {
                var newCharacter = await _characterRepository.CreateCharacterAsync(userId, charDto);

                return newCharacter;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create character", ex);
            }
        }
    }
}
