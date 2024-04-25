namespace FantasyChas_Backend.Models.DTOs
{
    public class CharacterDto
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; } /* vad har vi kontrollen?*/
        public int Level { get; set; }
        public int HealthPoints { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Constitution { get; set; }
        public int Charisma { get; set; }
        public string? Backstory { get; set; }

        // Ändra troligen till int. Beror på hur vi väljer att ta in från FE.
        public int ProfessionId { get; set; }
        public int SpeciesId { get; set; }
    }
}
