namespace FantasyChas_Backend.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string ChatSummary { get; set; }
        public virtual ActiveStory ActiveStory { get; set; }
        public virtual ICollection<ChatHistory> ChatHistory { get; set; }
    }
}
