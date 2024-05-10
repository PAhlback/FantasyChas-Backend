using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FantasyChas_Backend.Repositories
{
    public interface ICharacterRepository
    {
        public Task<List<CharacterViewModel>> GetCharactersForUser(string userId);
        public void AddCharacterToUser(Character newCharacter);
        public Task UpdateCharacter(int characterId, Character updatedCharacter);
        public Task DeleteCharacterAsync(string userId, int characterId);
    }

    public class CharacterRepository : ICharacterRepository
    {
        private static ApplicationDbContext _context;

        public CharacterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddCharacterToUser(Character newCharacter)
        {
            try
            {
                _context.Characters.Add(newCharacter);
                _context.SaveChanges();
            }
            catch
            {

            }
        }

        public async Task DeleteCharacterAsync(string userId, int characterId)
        {
            try
            {
                var characterToDelete = await _context.Characters
                                                      .Where(c => c.User.Id == userId && c.Id == characterId)
                                                      .SingleOrDefaultAsync();

                if (characterToDelete == null)
                {
                    throw new Exception($"Unable to delete character. Character with ID {characterId} not found.");
                }

                _context.Characters.Remove(characterToDelete);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<CharacterViewModel>> GetCharactersForUser(string userId)
        {
            try
            {
                var characters2 = _context.Characters
                    .Where(u => u.User.Id == userId);

                if (characters2.Count() == 0)
                {
                    throw new Exception("No characters found!");
                }

                List<CharacterViewModel> characters = await _context.Characters
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
                    .ToListAsync();

                return characters;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get characters");
            }
        }

        public async Task UpdateCharacter(int characterId, Character updatedCharacter)
        {
            var characterToUpdate = _context.Characters
                    .SingleOrDefault(u => u.Id == characterId && u.User.Id == updatedCharacter.User.Id);

            if (characterToUpdate is null)
            {
                throw new Exception("Character not found for active user!");
            }

            characterToUpdate.Name = updatedCharacter.Name;
            characterToUpdate.Age = updatedCharacter.Age;
            characterToUpdate.Gender = updatedCharacter.Gender;
            characterToUpdate.Level = updatedCharacter.Level;
            characterToUpdate.HealthPoints = updatedCharacter.HealthPoints;
            characterToUpdate.Strength = updatedCharacter.Strength;
            characterToUpdate.Dexterity = updatedCharacter.Dexterity;
            characterToUpdate.Intelligence = updatedCharacter.Intelligence;
            characterToUpdate.Wisdom = updatedCharacter.Wisdom;
            characterToUpdate.Constitution = updatedCharacter.Constitution;
            characterToUpdate.Charisma = updatedCharacter.Charisma;
            characterToUpdate.Backstory = updatedCharacter.Backstory;
            characterToUpdate.Profession = updatedCharacter.Profession;
            characterToUpdate.Species = updatedCharacter.Species;
            characterToUpdate.Favourite = updatedCharacter.Favourite;
            characterToUpdate.ImageURL = updatedCharacter.ImageURL;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not update character");
            }
        }
    }
}
