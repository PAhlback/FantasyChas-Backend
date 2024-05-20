namespace FantasyChas_Backend.Models.ViewModels
{
    public class NewCharacterViewModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } 
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Constitution { get; set; }
        public int Charisma { get; set; }
        public string? ImageURL { get; set; }
        public string Backstory { get; set; }
        public string Profession { get; set; }
        public string Species { get; set; }
    }
}
