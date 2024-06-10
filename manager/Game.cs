namespace manager
{
    public class Game
    {
        public string id { get; set; }
        public string type { get; set; }
        public Card[] boardcards { get; set; }
        public Card[] handcards { get; set; }
        public int opponent { get; set; }
    }
}
