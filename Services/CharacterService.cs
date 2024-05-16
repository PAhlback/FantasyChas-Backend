using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Repositories;
using FantasyChas_Backend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using FantasyChas_Backend.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Identity;

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
                var characters = await _characterRepository.GetCharactersForUserAsync(userId);

                if (characters.Count() == 0)
                {
                    throw new Exception("No characters found!");
                }

                List<CharacterViewModel> userCharacters = characters
                    .Select(c => new CharacterViewModel()
                    {
                        Id = c.Id,
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

        public async Task CreateCharacterAsync(IdentityUser user, CharacterDto charDto)
        {
            try
            {
                Character newCharacter = new Character()
                {
                    User = user,
                    Name = charDto.Name,
                    Age = charDto.Age,
                    Gender = charDto.Gender,
                    Level = charDto.Level,
                    HealthPoints = charDto.HealthPoints,
                    Strength = charDto.Strength,
                    Dexterity = charDto.Dexterity,
                    Intelligence = charDto.Intelligence,
                    Wisdom = charDto.Wisdom,
                    Constitution = charDto.Constitution,
                    Charisma = charDto.Charisma,
                    Backstory = charDto.Backstory,
                    Favourite = charDto.Favourite,
                    ImageURL = charDto.ImageURL,
                    Profession = charDto.Profession,
                    Species = charDto.Species
                };

                await _characterRepository.AddCharacterAsync(newCharacter);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create character", ex);
            }
        }

        public async Task UpdateCharacterAsync(IdentityUser user, CharacterWithIdDto charDto)
        {
            try
            {
                Character updatedCharacter = new Character()
                {
                    User = user,
                    Name = charDto.Name,
                    Age = charDto.Age,
                    Gender = charDto.Gender,
                    Level = charDto.Level,
                    HealthPoints = charDto.HealthPoints,
                    Strength = charDto.Strength,
                    Dexterity = charDto.Dexterity,
                    Intelligence = charDto.Intelligence,
                    Wisdom = charDto.Wisdom,
                    Constitution = charDto.Constitution,
                    Charisma = charDto.Charisma,
                    Backstory = charDto.Backstory,
                    Favourite = charDto.Favourite,
                    ImageURL = charDto.ImageURL,
                    Profession = charDto.Profession,
                    Species = charDto.Species
                };

                await _characterRepository.UpdateCharacterAsync(charDto.Id, updatedCharacter);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update character", ex);
            }
        }

        public async Task DeleteCharacterAsync(string userId, int characterId)
        {
            try
            {
                await _characterRepository.DeleteCharacterAsync(userId, characterId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete character", ex);
            }
        }
    }
}
