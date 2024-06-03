using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Repositories;
using FantasyChas_Backend.Services;
using FantasyChas_Backend.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OpenAI_API.Chat;

namespace FantasyChas_Backend.Services
{
    public interface ICharacterService
    {
        Task<List<CharacterViewModel>> GetCharactersForUser(string userId);
        Task CreateCharacterAsync(IdentityUser user, CharacterDto charDto);
        Task<string> CreateCharacterWithAiAsync(PromptDto promptDto);
        Task UpdateCharacterAsync(IdentityUser user, CharacterWithIdDto charDto);
        Task DeleteCharacterAsync(IdentityUser user, int charId);
        Task<bool> CharacterExistsAsync(int characterId, string userId);
        Task ConnectCharToStoryAsync(int characterId, int storyId, string userId);
        Task<CharacterViewModel> ConvertCharacterToViewModelAsync(Character character);
    }
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IOpenAiService _openAiService;
        private readonly IBlobStorageService _blobStorageService;

        public CharacterService(ICharacterRepository characterRepository, IOpenAiService openAiService, IBlobStorageService blobStorageService)
        {
            _characterRepository = characterRepository;
            _openAiService = openAiService;
            _blobStorageService = blobStorageService;
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
                string? imageUrl = null;
                if (!string.IsNullOrEmpty(charDto.ImageURL))
                {
                    imageUrl = await _blobStorageService.UploadImageFromUrlAsync(charDto.ImageURL);
                }
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
                    ImageURL = imageUrl,
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

                bool isImageTheSame = await _characterRepository.CheckCharacterImageUrl(charDto.Id, charDto.ImageURL);
                if (!isImageTheSame && !string.IsNullOrEmpty(charDto.ImageURL))
                {
                    updatedCharacter.ImageURL = await _blobStorageService.UploadImageFromUrlAsync(updatedCharacter.ImageURL);
                }
                await _characterRepository.UpdateCharacterAsync(charDto.Id, updatedCharacter);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update character", ex);
            }
        }

        public async Task DeleteCharacterAsync(IdentityUser user, int charId)
        {
            try
            {
                await _characterRepository.DeleteCharacterAsync(user, charId);
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

        public async Task<string> CreateCharacterWithAiAsync(PromptDto promptDto)
        {
            try
            {
                NewCharacterViewModel character = new NewCharacterViewModel();
                character.Gender = RandomValueGenerator.GetRandomGender();
                string randomName = RandomValueGenerator.GetRandomLetter().ToString();
                string randomProfession = RandomValueGenerator.GetRandomLetter().ToString();
                character.Age = RandomValueGenerator.GetRandomAge();

                var characterJson = JsonConvert.SerializeObject(character);

                string prompt = "";
                prompt = string.IsNullOrEmpty(promptDto.Message) ? "" : $"Speciella önskemål om karaktären: {promptDto.Message}";

                var chatMessages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, "Du är en skapare av karaktärer för en berättelse. " +
                    "Din uppgift är att skapa en karaktär baserat på inspiration från Dungeons and Dragons-reglerna. " +
                    $"Skapa ett förnamn som börjar med bokstaven {randomName} och som passar karaktärens kön. " +
                    "Sätt alltid art till Människa. " +
                    $"Sätt yrket till ett verkligt yrke som **måste** börja med bokstaven {randomProfession}. " +
                    "Fyll endast fält som har värde 'null' eller '0'. " +
                    "Det ska inte förekomma någon magi eller övernaturliga element. " +
                    "Använd standard array (15, 14, 13, 12, 10, 8) för att sätta karaktärens stats. " +
                    "Hitta på en bakgrundshistoria som matchar statsen och yrket som du tilldelat karaktären. Bakgrundshistorien ska vara på svenska." +
                    $"{prompt}" +
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
