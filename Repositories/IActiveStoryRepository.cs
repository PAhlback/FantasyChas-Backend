using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;

namespace FantasyChas_Backend.Repositories
{
    public interface IActiveStoryRepository
    {
        public Task AddStoryAsync(ActiveStory newStory);
    }

    public class ActiveStoryRepository : IActiveStoryRepository
    {
        private static ApplicationDbContext _context;

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
    }
}
