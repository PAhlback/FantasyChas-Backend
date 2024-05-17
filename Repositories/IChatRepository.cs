using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;

namespace FantasyChas_Backend.Repositories
{
    public interface IChatRepository
    {
        Task<Chat> GetChatByIdAsync(int chatId);
        Task<List<ChatHistory>> GetChatHistory(int characterId);
        Task<Chat> GetChat(int characterId);
        Task SaveChatHistoryMessageInDatabase(ChatHistory historyLine);
    }
    public class ChatRepository : IChatRepository
    {
        private static ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> GetChatByIdAsync(int chatId)
        {
            try
            {
                Chat? chat = await _context
                    .Chats
                    .SingleOrDefaultAsync(c => c.Id == chatId);

                return chat;
            }
            catch
            {
                throw new Exception();
            }
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

                return history;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<Chat> GetChat(int characterId)
        {
            try
            {
                var summary = await _context.ActiveStories
                    .Where(a => a.Characters.SingleOrDefault().Id == characterId)
                    .Include(a => a.Chats)
                    .Select(a => a.Chats
                                    .OrderByDescending(c => c.Id) // inte optimalt
                                    .FirstOrDefault()
                            )
                    .SingleOrDefaultAsync();

                return summary;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task SaveChatHistoryMessageInDatabase(ChatHistory historyLine)
        {
            try
            {
                await _context.AddAsync(historyLine);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception();
            }

        }

        public Task SaveChatHistoryMessageInDatabase(string message, int chatId, int? characterId)
        {
            throw new NotImplementedException();
        }
    }
}
