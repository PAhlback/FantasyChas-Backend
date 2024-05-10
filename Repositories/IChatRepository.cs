using FantasyChas_Backend.Data;

namespace FantasyChas_Backend.Repositories
{
    public interface IChatRepository
    {
        //public Task<ChatAnswerViewModel> GetAnswerFromChatGPT(ChatQueryDto query);
    }
    public class ChatRepository:IChatRepository
    {
        private static ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //public Task<ChatAnswerViewModel> GetAnswerFromChatGPT(ChatQueryDto query)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
