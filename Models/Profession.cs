namespace FantasyChas_Backend.Models
{
    public class Profession
    {
        public int Id { get; set; }
        public string? ProfessionName { get; set; }

        public virtual ICollection<Character> Characters { get; set; }
    }
}
