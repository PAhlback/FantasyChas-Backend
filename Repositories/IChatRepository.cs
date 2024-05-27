using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Cryptography;

namespace FantasyChas_Backend.Repositories
{
    public interface IChatRepository
    {
        Task AddChatAsync(Chat chat);
        Task<Chat> GetChatByIdAsync(int chatId);
        Task<List<ChatHistory>> GetChatHistoryAsync(int characterId);
        Task<Chat> GetChatAsync(int characterId);
        Task<List<ChatHistory>> GetPaginatedChatHistoryAsync(int activeStoryId, int amountPerPage, int pageNumber);
        Task SaveChatHistoryMessageInDatabaseAsync(ChatHistory historyLine);
    }

    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

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

        public async Task<List<ChatHistory>> GetChatHistoryAsync(int characterId)
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

        public async Task<Chat> GetChatAsync(int characterId)
        {
            try
            {
                // Den här fungerar troligen inte om det senare går att ha flera karaktärer på en story.
                var chat = await _context.ActiveStories
                    .Where(a => a.Characters.SingleOrDefault().Id == characterId)
                    .Include(a => a.Chats)
                    .Select(a => a.Chats
                                    .OrderByDescending(c => c.Id) // inte optimalt
                                    .FirstOrDefault()
                            )
                    .SingleOrDefaultAsync();

                return chat;
            }
            catch
            {
                throw new Exception("No chat found");
            }
        }

        public async Task SaveChatHistoryMessageInDatabaseAsync(ChatHistory historyLine)
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

        public async Task AddChatAsync(Chat chat)
        {
            try
            {
                await _context.AddAsync(chat);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Failed to add chat object");
            }
        }

        public async Task<List<ChatHistory>> GetPaginatedChatHistoryAsync(int activeStoryId, int amountPerPage, int pageNumber)
        {
            try
            {
                if(pageNumber - 1 >= 0)
                {
                    pageNumber--;
                }
                else
                {
                    throw new Exception("Page number has to be set to 1 or higher");
                }

                // Get all history
                var chatLines = await _context.Chats
                    .Where(c => c.ActiveStory.Id == activeStoryId)
                    .SelectMany(c => c.ChatHistory)
                    .OrderByDescending(ch => ch.Timestamp)
                    .Skip(pageNumber * amountPerPage)
                    .Take(amountPerPage)
                    .Include(ch => ch.Character)
                    .ToListAsync();

                if(chatLines.Count == 0)
                {
                    throw new Exception("No more messages found");
                }

                return chatLines;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task DeleteChatHistoryAsync(List<ChatHistory> chatHistories)
        {
            _context.ChatHistories.RemoveRange(chatHistories);
            await _context.SaveChangesAsync();
        }
    }
}
