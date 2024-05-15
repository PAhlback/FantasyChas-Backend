using Microsoft.AspNetCore.Identity;

namespace FantasyChas_Backend.Models
{
    public class SavedStory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
