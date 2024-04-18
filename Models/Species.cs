namespace FantasyChas_Backend.Models
{
    public class Species
    {
        public int Id { get; set; }
        public string SpeciesName { get; set; }

        public virtual ICollection<Character> Characters { get; set; }
    }
}
