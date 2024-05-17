using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FantasyChas_Backend.Repositories
{
    public interface IChatRepository
    {
        Task<List<ChatHistory>> GetChatHistory(int characterId);
        Task<string> GetChatSummary(int characterId);
        Task SaveChatHistoryMessageInDatabase(string message, int chatId, int? characterId);
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
                    .Where(a => a.Characters.Any(c => c.Id == characterId)) 
                    .SelectMany(a => a.Chats) 
                    .OrderByDescending(c => c.Id)  // inte optimalt
                    .Take(1) 
                    .SelectMany(c => c.ChatHistory) 
                    .ToListAsync();

                return chatHistory;
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
                .Select(a => a.Chats
                                .OrderByDescending(c => c.Id) // inte optimalt
                                .FirstOrDefault()
                                .ChatSummary
                        )
                .SingleOrDefaultAsync();

            List<Chat> chats = await _context.Chats
                .Where(c => c.ActiveStory == story)
                .ToListAsync();

            string summary = chats.LastOrDefault().ChatSummary;

            return summary;
        }

        public Task SaveChatHistoryMessageInDatabase(ChatHistory chatHistory)
        {
            
        }
    }
}
