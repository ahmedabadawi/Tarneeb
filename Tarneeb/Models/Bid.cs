using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarneeb.Models
{
    /// <summary>
    /// Represents player bid. and it can be
    /// - Number of tricks and the suit
    /// - Pass
    /// - Double / Redouble
    /// </summary>
    public class Bid : IComparable<Bid>
    {
        public bool IsPass { get; private set; }
        public bool IsDouble { get; set; }
        public int Tricks { get; private set; }
        public Suit Suit { get; private set; }
        public Player Player { get; private set; }

        /// <summary>
        /// Creates a bid object with the specified number of tricks
        /// and the specified suit
        /// </summary>
        public static Bid CreateBid(Player player, int tricks, Suit suit)
        {
            Bid bid = new Bid()
            {
                IsPass = false,
                Player = player,
                Tricks = tricks,
                Suit = suit
            };

            return bid;
        }

        /// <summary>
        /// Creates a bid object with Pass call
        /// </summary>
        public static Bid CreatePassBid(Player player)
        {
            Bid bid = new Bid()
            {
                IsPass = true,
                Player = player
            };

            return bid;
        }

        /// <summary>
        /// Create a bid object with Pass and Double call
        /// </summary>

        public static Bid CreatePassAndDoubleBid(Player player)
        {
            Bid bid = CreatePassBid(player);

            bid.IsDouble = true;

            return bid;
        }

        private Bid()
        {

        }

        public override string ToString()
        {
            if (IsPass)
            {
                return string.Format("{0} - Pass", Player.ToString());
            }
            else
            {
                string suitString =
                    (Suit == Suit.None) ? "No Trump" : Suit.ToString();

                return string.Format("{0} - {1} - {2}", Player.ToString(), Tricks, suitString);
            }

        }

        /// <summary>
        /// Compares two bids according the number of tricks and suit power
        /// </summary>
        public int CompareTo(Bid other)
        {
            //TODO: Handle pass in the comparison
            if (this.Tricks != other.Tricks)
            {
                return this.Tricks.CompareTo(other.Tricks);
            }
            else
            {
                if (this.Suit == other.Suit)
                {
                    return 0;
                }
                else
                {
                    int thisSuitOrder = 4 - (int)this.Suit;
                    int otherSuitOrder = 4 - (int)other.Suit;

                    return thisSuitOrder.CompareTo(otherSuitOrder);
                }
            }
        }
    }
}