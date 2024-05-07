namespace FantasyChas_Backend.Models.DTOs
{
    public class CharacterWithIdDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public int Level { get; set; }
        public int HealthPoints { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Constitution { get; set; }
        public int Charisma { get; set; }
        public string? Backstory { get; set; }

        public int ProfessionId { get; set; }
        public int SpeciesId { get; set; }
    }
}
