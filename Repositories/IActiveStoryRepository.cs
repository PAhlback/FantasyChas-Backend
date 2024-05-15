using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;

namespace FantasyChas_Backend.Repositories
{
    public interface IActiveStoryRepository
    {
        public Task AddActiveStoryAsync(ActiveStory newactiveStory);
    }

    public class ActiveStoryRepository : IActiveStoryRepository
    {
        private static ApplicationDbContext _context;

        public ActiveStoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddActiveStoryAsync(ActiveStory newactiveStory)
        {
            try
            {
                _context.ActiveStories.Add(newactiveStory);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create activeStory", ex);
            }
        }
    }
}
