namespace frontend.Models
{
    public class GameType
    {
        public string id { get; set; }
        public string name { get; set; }
        public int handcards { get; set; }
        public int boardcards { get; set; }

        public GameType(string name, int hand, int board)
        {
            this.name = name;
            this.handcards = hand;
            this.boardcards = board;
        }
    }
}
