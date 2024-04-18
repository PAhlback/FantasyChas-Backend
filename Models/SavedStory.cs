namespace FantasyChas_Backend.Models
{
    public class SavedStory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }

        public virtual Character Character { get; set; }
    }
}
