using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Repositories;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using OpenAI_API.Chat;

namespace FantasyChas_Backend.Services
{
    public interface ICharacterService
    {
        Task<List<CharacterViewModel>> GetCharactersForUser(string userId);
        Task CreateCharacterAsync(IdentityUser user, CharacterDto charDto);
        Task<string> CreateCharacterWithAiAsync(NewCharacterViewModel newCharacter);
        Task UpdateCharacterAsync(IdentityUser user, CharacterWithIdDto charDto);
        Task DeleteCharacterAsync(string userId, int CharacterId);
        Task<bool> CharacterExistsAsync(int characterId, string userId);
        Task ConnectCharToStoryAsync(int characterId, int storyId, string userId);
        Task<CharacterViewModel> ConvertCharacterToViewModelAsync(Character character);
    }
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IOpenAiService _openAiService;

        public CharacterService(ICharacterRepository characterRepository, IOpenAiService openAiService)
        {
            _characterRepository = characterRepository;
            _openAiService = openAiService;
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
                        Species = c.Species,
                        Favourite = c.Favourite,
                        ImageURL = c.ImageURL
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

        public async Task ConnectCharToStoryAsync(int characterId, int storyId, string userId)
        {
            try
            {
                await _characterRepository.ConnectCharToStoryAsync(characterId, storyId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to connect character to story", ex);
            }
        }

        public async Task<bool> CharacterExistsAsync(int characterId, string userId)
        {
            return await _characterRepository.CharacterExistsAsync(characterId, userId);
        }

        public async Task<CharacterViewModel> ConvertCharacterToViewModelAsync(Character character)
        {
            try
            {
                CharacterViewModel? characterViewModel = new CharacterViewModel()
                {
                    Id = character.Id,
                    Name = character.Name,
                    Age = character.Age,
                    Gender = character.Gender,
                    Level = character.Level,
                    HealthPoints = character.HealthPoints,
                    Strength = character.Strength,
                    Dexterity = character.Dexterity,
                    Intelligence = character.Intelligence,
                    Wisdom = character.Wisdom,
                    Constitution = character.Constitution,
                    Charisma = character.Charisma,
                    Favourite = character.Favourite,
                    ImageURL = character.ImageURL,
                    Backstory = character.Backstory,
                    Profession = character.Profession,
                    Species = character.Species
                };
                return characterViewModel;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<string> CreateCharacterWithAiAsync(NewCharacterViewModel character)
        {
            try
            {
                var characterJson = JsonConvert.SerializeObject(character);

                var chatMessages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, "Du är en Dungeon Master för spelet Dungeons and Dragons. " +
                    "Din uppgift är att skapa en karaktär för spelet baserat på reglerna i Dungeons and Dragons. " +
                    "Skapa ett nytt namn varje gång. " +
                    "Sätt ålder mellan 10 - 9000. " +
                    "Sätt gender till Male, Female eller Non-binary. " +
                    "Sätt alltid species till Human. " +
                    "Sätt profession till ett slumpmässigt yrke. " +
                    "Fyll endast fält som har värde 'null' eller '0'. " +
                    "Hitta på en backstory som matchar statsen och yrket som du tilldelat karaktären. Backstoryn ska vara på svenska." +
                    "Svara i JSON-format."),
                    new ChatMessage(ChatMessageRole.User, $"Min karaktär: {characterJson}"),
                };

                var result = await _openAiService.GetChatGPTResultAsync(chatMessages);

                if (result == null)
                {
                    throw new Exception("AI service returned null response.");
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the character.", ex);
            }
        }
    }
}
