using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using contracts;
using System.ComponentModel.Composition;

namespace texasholdem
{
    [Export(typeof(IGameLogic))]
    [ExportMetadata("Name", "texasholdem")]
    public class Gamelogic : IGameLogic
    {

        public double CalculateWinProbability(Game input)
        {

            StreamReader r = new StreamReader("./TexasHoldem/CardCombisObjects.json");
            string json = r.ReadToEnd();
            List<CardCombi> items = JsonConvert.DeserializeObject<List<CardCombi>>(json);
            List<CardCombi> possibleItems = filterPossibleCombis(input, items);

            return winProb(possibleItems, input);
        }

        private double winProb(List<CardCombi> possibleHands, Game game) //Opponent count integrated in game object andd not implemented
        {
            int ownScore = scoreOwnHand(game);
            int[] possibleHandsScore = scoreOpponentHands(possibleHands, game.boardcards);
            int counter = 0;
            foreach(int element in possibleHandsScore) {
                if (element < ownScore) {
                    counter++;
                }
            }
            return (double) counter / possibleHandsScore.Length; 
            //TODO check own Score vs opponentScores
            // return Vergleich zwischen händen mit niedrigerem score als der eigene score und allen händen
        }

        private int scoreOwnHand(Game game)
        {
            return calcScore(game.handcards, game.boardcards);
        }
        private int[] scoreOpponentHands(List<CardCombi> possibleHands, Card[] boardCards)
        {
            int[] result = new int[possibleHands.Count];
            //TODO loop calcScore mit allen möglichkeiten
            for (int i = 0; i < possibleHands.Count; i++)
            {
                Card[] handCards = convertCardCombiToCard(possibleHands[i]);
                result[i] = calcScore(handCards, boardCards);
            }

            return result;
        }

        private Card[] convertCardCombiToCard(CardCombi input)
        {
            Card[] handCards = new Card[2];
            int subStringLength = 0;
            switch (input.first.Substring(0, 1))
            {
                case "c":
                    subStringLength = 5;
                    handCards[0].cardType = "clubs";
                    break;
                case "d":
                    subStringLength = 8;
                    handCards[0].cardType = "diamonds";
                    break;

                case "h":
                    subStringLength = 5;
                    handCards[0].cardType = "heart";
                    break;

                case "s":
                    subStringLength = 6;
                    handCards[0].cardType = "spades";
                    break;
            }
            handCards[0].cardValue = input.first.Substring(subStringLength, input.first.Length);


            switch (input.second.Substring(0, 1))
            {
                case "c":
                    subStringLength = 5;
                    handCards[1].cardType = "clubs";
                    break;
                case "d":
                    subStringLength = 8;
                    handCards[1].cardType = "diamonds";
                    break;

                case "h":
                    subStringLength = 5;
                    handCards[1].cardType = "heart";
                    break;

                case "s":
                    subStringLength = 6;
                    handCards[1].cardType = "spades";
                    break;
            }
            handCards[1].cardValue = input.second.Substring(subStringLength, input.second.Length);

            return handCards;
        }

        private int calcScore(Card[] handcards, Card[] boardCards)
        {
            int result = 0;
            bool straightBool = false;
            bool flushBool = false;


            Card[] knownCards = new Card[handcards.Length + boardCards.Length];
            boardCards.CopyTo(knownCards, 0);
            handcards.CopyTo(knownCards, boardCards.Length);

            List<Card>[] cards = organizeCards(knownCards);
            List<Card> straightCards = new List<Card>();
            int hit = 0;

            for (int i = 0; i < cards.Length; i++)
            {
                if (hit < 5)
                {
                    if (cards[i].Count != 0)
                    {
                        straightCards.AddRange(cards[i]);
                        hit++;
                    }
                    else
                    {
                        straightCards = new List<Card>();
                        hit = 0;
                    }
                }
                else
                {
                    result = 40000;
                    straightBool = true;
                }
            }
            //Flush check

            int[] flush = new int[4];
            foreach (Card element in knownCards)
            {
                checkFlush(element, ref flush);
            }

            for (int i = 0; i < flush.Length && !flushBool; i++)
            {
                flushBool = flush[i] >= 5;
            }

            if (flushBool)
            {
                result = 50000;
            }


            if (straightBool && flushBool)
            {
                //straightFlush
                string firstColor = straightCards[0].cardType;
                bool noErrors = true;
                for (int i = 1; i < straightCards.Count && noErrors; i++)
                {
                    if (firstColor != straightCards[i].cardType)
                    {
                        noErrors = false;
                    }
                }
                if (noErrors)
                {
                    result = 80000;
                }


                //royalFlush
                int[] royalFlush = new int[4];

                foreach (Card element in knownCards)
                {
                    if (element.cardValue.Equals("10") | element.cardValue.Equals("J") | element.cardValue.Equals("Q")
                    | element.cardValue.Equals("K") | element.cardValue.Equals("A"))
                    {
                        checkFlush(element, ref royalFlush);
                    }
                }
                bool royalFlushChance = false;

                for (int i = 0; i < royalFlush.Length && !royalFlushChance; i++)
                {
                    royalFlushChance = royalFlush[i] >= 5;
                }
                if (royalFlushChance)
                {
                    result = 90000;
                }
            }

            //vierling
            if (result <= 70000)
            {
                foreach (List<Card> cardList in cards)
                {
                    if (cardList.Count >= 4)
                    {
                        result = 70000;
                    }
                }
            }
            bool twoHit = false;
            //full house & drilling
            if (result <= 60000)
            {
                bool threeHit = false;
                foreach (List<Card> cardList in cards)
                {
                    if (cardList.Count == 3 && !threeHit)
                    {
                        threeHit = true;
                    }
                    if (cardList.Count == 2 && !twoHit)
                    {
                        twoHit = true;
                    }
                }
                if (twoHit && threeHit)
                {
                    result = 60000;
                }
                else if (threeHit && result <= 30000) //drilling
                {
                    result = 30000;
                }
            }
            //two pair
            //pair
            int pairs = 0;
            if (result <= 20000 && twoHit)
            {
                foreach (List<Card> cardList in cards)
                {
                    if (cardList.Count == 2)
                    {
                        pairs++;
                    }
                }
            }
            if (pairs >= 2)
            {
                result = 20000;
            }
            else if (pairs == 1 && result < 10000)
            {
                result = 10000;
            }

            bool highCardIsSet = false;
            //high card für alle hände
            for (int i = cards.Length - 1; i > 0 && !highCardIsSet; i--)
            {
                if (cards[i].Count > 0)
                {
                    result += i;
                    highCardIsSet = true;
                }

            }
            return result;
        }


        //Liefert eine List mit möglichen Karten zurück indem alle Combis aus Json mit den bekannten Karten verglichen werden
        private List<CardCombi> filterPossibleCombis(Game game, List<CardCombi> allHands)
        {
            Card[] knownCards = new Card[game.handcards.Length + game.boardcards.Length];

            game.handcards.CopyTo(knownCards, 0);
            game.boardcards.CopyTo(knownCards, game.handcards.Length);


            List<CardCombi> result = new List<CardCombi>();
            List<CardCombi> allCards = new List<CardCombi>();
            string tempFirst = "";
            string tempSecond = "";
            //Die KnownCards müssen umgewandelt werden da die json datei herz zwei z.B. als "H2" hat und ein Card Element als "Heart2"
            for (int i = 0; i < allHands.Count; i++)
            {
                switch (allHands[i].first.Substring(0, 1))
                {

                    case "C":
                        tempFirst = "clubs";
                        break;
                    case "D":
                        tempFirst = "diamonds";
                        break;

                    case "H":
                        tempFirst = "heart";
                        break;

                    case "S":
                        tempFirst = "spades";
                        break;
                }
                switch (allHands[i].second.Substring(0, 1))
                {

                    case "C":
                        tempSecond = "clubs";
                        break;
                    case "D":
                        tempSecond = "diamonds";
                        break;

                    case "H":
                        tempSecond = "heart";
                        break;

                    case "S":
                        tempSecond = "spades";
                        break;
                }
                CardCombi tempCombi = new CardCombi();
                tempCombi.first = tempFirst + allHands[i].first.Substring(1, allHands[i].first.Length - 1);
                tempCombi.second = tempSecond + allHands[i].second.Substring(1, allHands[i].second.Length - 1);
                allCards.Add(tempCombi);
            }
            //Es werden alle kombinationen mit den bekannten Karten verglichen um die auszuschließen, welche schon bekannt sind
            foreach (CardCombi element in allCards)
            {
                for (int i = 0; i < knownCards.Length; i++)
                {
                    if (!((element.first).Equals(knownCards[i].cardType + knownCards[i].cardValue) || (element.second).Equals(knownCards[i].cardType + knownCards[i].cardValue)))
                    {
                        result.Add(element);
                    }
                }
            }
            return result;
        }
        //Check ob Flush möglich ist -> Flush, Straight Flush und Royal Flush können vlt. ausgeschlossen werden
        //Array mit 0-3 für Kreuz - Karo - Herz - Pik hochzählen mit foreach. Wenn eins größer gleich 3 ist besteht flushgefahr
        private void checkFlush(Card element, ref int[] flush)
        {

            switch (element.cardType)
            {

                case "clubs":
                    flush[0]++;
                    break;
                case "diamonds":
                    flush[1]++;
                    break;

                case "heart":
                    flush[2]++;
                    break;

                case "spades":
                    flush[3]++;
                    break;
            }
        }

        //Check ob Straight möglich ist -> Royal Flush, Straight Flush und Straße(Straight) können vlt. ausgeschlossen werden
        //Array mit länge der anzahl der Karten(Ass = 0 und 13, 2 = 1, 3 = 2, ... , König = 12) karten bei der forschleife durchgehen
        //und zahl im array um einen erhöhen. Danach array durchgehen. Anfangen zu zählen bei der ersten 1 die man trifft und 
        //dann kann man in der nächsten 5 max 2 leere felder isg. und hintereinander haben und auf 3 hochzählen pro vollem feld.
        private List<Card>[] organizeCards(Card[] cards)
        {
            List<Card>[] result = new List<Card>[14];
            foreach (Card element in cards)
            {
                switch (element.cardType)
                {

                    case "A":
                        result[0].Add(element);
                        result[13].Add(element);
                        break;
                    case "2":
                        result[1].Add(element);
                        break;
                    case "3":
                        result[2].Add(element);
                        break;
                    case "4":
                        result[3].Add(element);
                        break;
                    case "5":
                        result[4].Add(element);
                        break;
                    case "6":
                        result[5].Add(element);
                        break;
                    case "7":
                        result[6].Add(element);
                        break;
                    case "8":
                        result[7].Add(element);
                        break;
                    case "9":
                        result[8].Add(element);
                        break;
                    case "10":
                        result[9].Add(element);
                        break;
                    case "J":
                        result[10].Add(element);
                        break;
                    case "Q":
                        result[11].Add(element);
                        break;
                    case "K":
                        result[12].Add(element);
                        break;
                }
            }
            return result;
        }
    }
}
