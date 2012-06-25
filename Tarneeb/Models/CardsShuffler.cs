using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarneeb.Models
{
    /// <summary>
    /// Encapsulates the shuffling algorithm
    /// </summary>
    public class CardsShuffler
    {
        private const int numberOfPlayers = 4;

        /// <summary>
        /// Shuffles the cards and returns list of lists that represent
        /// the hand of each player.
        /// Note: the hand is sorted from lowest card to the highest card and 
        /// suits are separated
        /// </summary>
        public List<List<Card>> Shuffle()
        {
            List<List<Card>> shuffledCards = new List<List<Card>>();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                shuffledCards.Add(new List<Card>());
            }

            List<Card> allCards = AllCards;
            Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            for (int i = allCards.Count - 1; i >= 1; i--)
            {
                int j = random.Next(0, i);
                Card tempCard = allCards[j];
                allCards[j] = allCards[i];
                allCards[i] = tempCard;
            }


            int tempCount = 1;
            int tempPlayer = 1;
            int cardsPerPlayer = (int)((float)allCards.Count / (float)numberOfPlayers);
            foreach (Card card in allCards)
            {
                if (tempCount > cardsPerPlayer)
                {
                    tempCount = 1;
                    tempPlayer++;
                }

                shuffledCards[tempPlayer - 1].Add(card);

                tempCount++;
            }
            
            shuffledCards.ForEach(playerCards => playerCards.Sort());

            return shuffledCards;
        }
        
        /// <summary>
        /// Generates the un-shuffled cards
        /// </summary>
        private List<Card> AllCards
        {
            get
            {
                List<Card> allCards = new List<Card>();
                for (int i = (int)Suit.Spades; i <= (int)Suit.Clubs; i++)
                {
                    for (int j = 1; j <= 13; j++)
                    {
                        Card card = new Card(j, (Suit)i);
                        allCards.Add(card);
                    }
                }

                return allCards;
            }
        }
    }
}