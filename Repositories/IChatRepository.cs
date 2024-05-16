using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FantasyChas_Backend.Repositories
{
    public interface IChatRepository
    {
        public Task<List<ChatHistory>> GetChatHistory(int characterId);
        public Task<string> GetChatSummary(int characterId);
    }
    public class ChatRepository : IChatRepository
    {
        private static ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatHistory>> GetChatHistory(int characterId)
        {
            try
            {
                var history = await _context.ActiveStories
                //.Where(a => a.Characters.SingleOrDefault().Id == characterId)
                .Include(a => a.Chats)
                .SelectMany(c => c.Chats.ChatHistory)
                .ToListAsync();

                return history;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<string> GetChatSummary(int characterId)
        {
            var summary = await _context.ActiveStories
                .Where(a => a.Characters.SingleOrDefault().Id == characterId)
                .Include(a => a.Chats)
                .Select(a => a.Chats.LastOrDefault().ChatSummary)
                .SingleOrDefaultAsync();

            return summary;
        }
    }
}
