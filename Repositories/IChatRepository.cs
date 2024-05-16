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
                ////var history = await _context.ActiveStories
                ////.Where(a => a.Characters.SingleOrDefault().Id == characterId)
                ////.Include(a => a.Chats)
                ////.SelectMany(c => c.Chats.ChatHistory)
                ////.ToListAsync();

                //ActiveStory? story = await _context.ActiveStories
                //    .Where(a => a.Characters.FirstOrDefault().Id == characterId)
                //    .SingleOrDefaultAsync();

                //List<Chat> chats = await _context.Chats
                //    .Where(c => c.ActiveStory == story)
                //    .ToListAsync();

                //List<ChatHistory> chatHistory = new List<ChatHistory>();
                //foreach (var chat in chats)
                //{
                //    if(chat.ChatHistory != null)
                //    {
                //        foreach (var ch in chat.ChatHistory)
                //        {
                //            chatHistory.Add(ch);
                //        }
                //    }
                //}

                //return chatHistory;
                var chatHistory = await _context.Chats
                    .Include(c => c.ChatHistory)
                    .Where(c => c.ActiveStory.Characters.Any(ch => ch.Id == characterId))
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
            ActiveStory? story = await _context.ActiveStories
                    .Where(a => a.Characters.FirstOrDefault().Id == characterId)
                    .SingleOrDefaultAsync();

            List<Chat> chats = await _context.Chats
                .Where(c => c.ActiveStory == story)
                .ToListAsync();

            string summary = chats.LastOrDefault().ChatSummary;

            return summary;
        }
    }
}
