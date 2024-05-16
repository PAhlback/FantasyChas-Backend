namespace FantasyChas_Backend.Models
{
    public class ChatHistory
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual Character? Character { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
