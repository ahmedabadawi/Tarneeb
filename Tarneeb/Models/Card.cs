using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarneeb.Models
{
    /// <summary>
    /// This class represents a playing card, each playing card has Rank and Suit
    /// Rank: 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 (Jack), 12 (Queen), 13 (King), 1 (Ace)
    /// Suit: Spades (1), Hearts (2), Diamonds (3), Clubs (4), also None (0) - Not
    /// valid for cards.
    /// Note: The class implements IComparable to allow for sorting
    /// </summary>
    public class Card : IComparable<Card>
    {
        /// <summary>
        /// The card rank
        /// </summary>
        public int Rank { get; private set; }
        /// <summary>
        /// The card suit (S,H,D,C)
        /// </summary>
        public Suit Suit { get; private set; }

        /// <summary>
        /// Constructor that creates the object from the rank and suit
        /// </summary>
        public Card(int card, Suit suit)
        {
            this.Rank = card;
            this.Suit = suit;
        }

        /// <summary>
        /// Constructor that creates the object from string representation
        /// in the format of [Rank][Suit] for example AS => Ace of Spades, 10C => 10 of Clubs
        /// </summary>
        /// <param name="card"></param>
        public Card(string card)
        {
            string tempCard, tempSuit;
            tempCard = card.Substring(0, card.Length - 1);
            tempSuit = card.Substring(card.Length - 1, 1);

            this.Rank = int.Parse(tempCard);
            this.Suit = SuitFromString(tempSuit);
        }

        /// <summary>
        /// Compares two card objects according to the suit and rank
        /// </summary>
        public int CompareTo(Card other)
        {
            if (this.Suit != other.Suit)
            {
                return ((int)this.Suit).CompareTo((int)other.Suit) * -1;
            }
            else
            {
                if (this.Rank != 1 && other.Rank != 1)
                {
                    return this.Rank.CompareTo(other.Rank);
                }
                else
                {
                    if (this.Rank == 1)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }
        
        /// <summary>
        /// Returns string representation of the card in the format of
        /// [Rank] [Suit] for example A Spades and 10 Diamonds
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string cardString;
            switch (Rank)
            {
                case 1:
                    cardString = "A";
                    break;
                case 11:
                    cardString = "J";
                    break;
                case 12:
                    cardString = "Q";
                    break;
                case 13:
                    cardString = "K";
                    break;
                default:
                    cardString = Rank.ToString();
                    break;
            }

            return string.Format("{0} {1}",cardString, Suit.ToString());
        }

        public static Suit SuitFromString(string suitString)
        {
            switch (suitString.ToUpper())
            {
                case "S":
                    return Suit.Spades;
                case "H":
                    return Suit.Hearts;
                case "D":
                    return Suit.Diamonds;
                case "C":
                    return Suit.Clubs;
                default:
                    throw new ArgumentException(string.Format("Suit {0} is not a valid suit.", suitString));
            }
        }
    }

    /// <summary>
    /// Enumeration of the possible suits
    /// Note: the None element is added to allow for usage in the bidding as no-trump
    /// </summary>
    public enum Suit
    {
        None = 0,
        Spades = 1,
        Hearts,
        Diamonds,
        Clubs
    }
}