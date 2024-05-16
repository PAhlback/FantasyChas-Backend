using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FantasyChas_Backend.Repositories
{
    public interface ICharacterRepository
    {
        public Task<List<Character>> GetCharactersForUserAsync(string userId);
        public Task AddCharacterAsync(Character newCharacter);
        public Task UpdateCharacterAsync(int characterId, Character updatedCharacter);
        public Task DeleteCharacterAsync(string userId, int characterId);
    }

    public class CharacterRepository : ICharacterRepository
    {
        private static ApplicationDbContext _context;

        public CharacterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Character>> GetCharactersForUserAsync(string userId)
        {
            try
            {
                var characters = await _context.Characters
                                               .Where(u => u.User.Id == userId)
                                               .ToListAsync();

                return characters;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get characters", ex);
            }
        }

        public async Task AddCharacterAsync(Character newCharacter)
        {
            try
            {
                _context.Characters.Add(newCharacter);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create character", ex);
            }
        }

        public async Task UpdateCharacterAsync(int characterId, Character updatedCharacter)
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
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not update character");
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
    }
}
