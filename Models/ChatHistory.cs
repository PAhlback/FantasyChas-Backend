namespace FantasyChas_Backend.Models
{
    public class ChatHistory
    {
        public int Id { get; set; }
        public string? Prompt { get; set; }
        public string? Answer { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Character? Character { get; set; }
        public virtual ActiveStory? ActiveStory { get; set; }
    }
}
