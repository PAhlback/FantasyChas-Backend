using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace FantasyChas_Backend.Repositories
{
    public interface IActiveStoryRepository
    {
        public Task AddStoryAsync(ActiveStory newStory);
        Task<ActiveStory> GetStoryByIdAsync(int activeStoryId);
    }

    public class ActiveStoryRepository : IActiveStoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ActiveStoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddStoryAsync(ActiveStory newStory)
        {
            try
            {
                _context.ActiveStories.Add(newStory);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create story", ex);
            }
        }

        public async Task<ActiveStory> GetStoryByIdAsync(int activeStoryId)
        {
            try
            {
                ActiveStory? story = await _context
                    .ActiveStories
                    .SingleOrDefaultAsync(aS => aS.Id == activeStoryId);

                return story;
            }
            catch
            {
                throw new Exception($"No active story with id {activeStoryId} found.");
            }
        }
    }
}
