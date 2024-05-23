namespace FantasyChas_Backend.Models.ViewModels
{
    public class ChatHistoryViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        //public virtual Chat Chat { get; set; }
        public string? CharacterName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
