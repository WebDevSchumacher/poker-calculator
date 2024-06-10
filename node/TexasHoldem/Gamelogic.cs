using System;
using System.Text.Json;
using node;

namespace node.TexasHoldem
{
    public class Gamelogic
    {
        public double CalculateWinProbability(Game input) {
            bool[] possibleBoardCombination = checkCombo(input.boardcards);
            return 1.0;
        }

        //checkCombo nimmt ein Array von Cards und liefert ein bool array zurück bei dem gilt
        //index 0 für Royal Flush wertig absteigend bis nach 5 für Straße/Straight
        private bool[] checkCombo(Card[] input) {
            //von index 0 für Royal Flush
            //1 Straight Flush
            //2 Vierling
            //3 Full House
            //4 Flush
            //5 Straße

            bool[] result = new bool[6];

            int[] flush = new int[4];
            int[] straight = new int[14];
            bool pairOnBoard = false;

            for(int i = 0; i < input.Length; i++) {
                Card element = input[i];

                checkFlush(element,ref flush);
                checkStraight(element,ref straight);

                //Check auf Full House und Vierling (auf paar auf board checken)
                //Checkt mit ner weiteren For schleife durch die restlichen durch und guckt ob sie gleich sind. Setzt eine Bool Variable
                //auf true dann wird auch die schleife gestoppt

                for(int j = i; j < input.Length && !pairOnBoard; j++) {
                    pairOnBoard = element.cardType.Equals(input[j]);
                }
            }

            //Wenn 3 oder mehr karten einer farbe liegen ist flushChance true das macht flush 100%tig möglich und Royal Flush und Straight Flush möglich
            bool flushChance = false;

            for(int i = 0; i < flush.Length && !flushChance; i++) {
                flushChance = flush[i] >= 3;
            }

            if(flushChance) {
                result[4] = true; //Flush Möglichkeit auf true
            }


            //durchs array durchgehen. Anfangen zu zählen bei der ersten 1 die man trifft und 
            //dann kann man in der nächsten 5 max 2 leere felder isg. und hintereinander haben und auf 3 hochzählen pro vollem feld.

            int hit = 0;

            foreach(int element in straight) {
                int miss = 0;
                if (hit < 3 && miss < 3) {
                    if (element != 0) {
                        hit++;
                    } else {
                        miss++;
                    }
                } else if(miss > 2) {
                    hit = 0;
                }

            }

            if(hit > 2) {
                if(flushChance) {
                    result[1] = true; //Straight Flush Möglichkeit auf true
                }

                result[5] = true; //Straße Möglichkeit auf true
            }


            //Extra check für Royal Flush. Es müssen 3 von den 5 höchsten karten liegen in der gleichen farbe
            //Erst check für die Karten
            if(flushChance) {
                int royalHit = 0;
            for(int i = 9; i < straight.Length; i++) {
                if(straight[i] > 0) {
                    royalHit++;
                }
            }

            //Dann wenn die karten da sind werden sie auf die farbe überprüft.

            if (royalHit >= 3) {
                int[] royalFlush = new int[4];

                foreach(Card element in input) {

                    if(element.cardValue.Equals("10") | element.cardValue.Equals("J") | element.cardValue.Equals("Q") 
                    | element.cardValue.Equals("K") | element.cardValue.Equals("A")) {

                        checkFlush(element, ref royalFlush);

                    }

                }

                bool royalFlushChance = false;

                for(int i = 0; i < flush.Length && !royalFlushChance; i++) {
                    royalFlushChance = flush[i] >= 3;
                }

                if(royalFlushChance) {
                    result[0] = true; //Royal Flush Möglichkeit auf true
                }
            }
            }


            //Wenn Pair auf dem Board ist, ist Full House und vierling möglich
            if(pairOnBoard) {

                result[2] = true; //Vierling Möglichkeit auf true
                result[3] = true; //Full House Möglichkeit auf true

            }
            

            return result;
        }

        //Check ob Flush möglich ist -> Flush, Straight Flush und Royal Flush können vlt. ausgeschlossen werden
        //Array mit 0-3 für Kreuz - Karo - Herz - Pik hochzählen mit foreach. Wenn eins größer gleich 3 ist besteht flushgefahr
        private void checkFlush(Card element,ref int[] flush) {

                    switch(element.cardType) {

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
        private void checkStraight(Card element, ref int[] straight) {

                    switch(element.cardType) {

                    case "A":
                        straight[0]++;
                        straight[13]++;
                        break;
                    case "2":
                        straight[1]++;
                        break;
                    case "3":
                        straight[2]++;
                        break;
                    case "4":
                        straight[3]++;
                        break;
                    case "5":
                        straight[4]++;
                        break;
                    case "6":
                        straight[5]++;
                        break;
                    case "7":
                        straight[6]++;
                        break;
                    case "8":
                        straight[7]++;
                        break;
                    case "9":
                        straight[8]++;
                        break;
                    case "10":
                        straight[9]++;
                        break;
                    case "J":
                        straight[10]++;
                        break;
                    case "Q":
                        straight[11]++;
                        break;
                    case "K":
                        straight[12]++;
                        break;
                }
        }
    }
}
