using System;

namespace frontend.Models
{
    public enum CardType
    {
        Heart = 1, Diamond = 2, Spade = 3, Club = 4
    }

    public class Card
    {
        //public String CardType { get; set; }
        public CardType CardType { get; set; }
        public String CardValue { get; set; }
        public Card(CardType cardType, String cardVame)
        {
            CardType = cardType;
            CardValue = cardVame;
        }
    }

    public class CardCardTypeString
    {
        public String CardType { get; set; }
        public String CardValue { get; set; }
        public CardCardTypeString(String cardType, String cardVame)
        {
            CardType = cardType;
            CardValue = cardVame;
        }
    }

}
