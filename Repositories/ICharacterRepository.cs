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
        public void UpdateCharacter(Character updateThisCharacter);
        public void DeleteCharacter(Character deleteThisCharacter);
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

        public void DeleteCharacter(Character deleteThisCharacter)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CharacterViewModel>> GetCharactersForUser(string userId)
        {
            try
            {
                var characters2 = _context.Characters
                    .Where(u => u.UserId == userId);

                if (characters2.Count() == 0)
                {
                    throw new Exception("No characters found!");
                }

                List<CharacterViewModel> characters = await _context.Characters
                    .Where(u => u.UserId == userId)
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
                        ProfessionName = c.Profession.ProfessionName,
                        SpeciesName = c.Species.SpeciesName
                    })
                    .ToListAsync();

                return characters;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get characters");
            }
        }

        public void UpdateCharacter(Character updateThisCharacter)
        {
            throw new NotImplementedException();
        }
    }
}
