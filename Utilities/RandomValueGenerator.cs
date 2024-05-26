namespace FantasyChas_Backend.Utilities
{
    public class RandomValueGenerator
    {
        private static readonly string[] Genders = { "Man", "Kvinna", "Icke-binär" };
        private static readonly Random Random = new Random();
        private static readonly char[] Letters = Enumerable.Range('A', 26).Select(x => (char)x).ToArray();

        public static string GetRandomGender()
        {
            return Genders[Random.Next(Genders.Length)];
        }

        public static char GetRandomLetter()
        {
            return Letters[Random.Next(Letters.Length)];
        }

        public static int GetRandomAge()
        {
            return Random.Next(16, 101); // 16 to 100 inclusive
        }
    }
}
