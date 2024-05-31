namespace FantasyChas_Backend.Models.ViewModels
{
    public class ImageViewModel
    {
        public ICollection<string>? Urls { get; set; }
        public DateTime? ExpiryTime { get; set; }
    }
}
