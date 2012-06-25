using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarneeb.Models
{
    /// <summary>
    /// Handles one stage of bidding, the bidding object is created upon changing
    /// the state of the game to Bidding state to handle the bidding, and is 
    /// destroyed when the bidding process is complete and game moved to the
    /// Game Play state
    /// </summary>
    class Bidding
    {
        /// <summary>
        /// Reference to the owner game session for callback
        /// </summary>
        private GameSession gameSession;

        /// <summary>
        /// List of bids
        /// </summary>
        public List<Bid> Bids { get; private set; }

        /// <summary>
        /// The current turn in the bidding
        /// </summary>
        public PlayerPosition CurrentTurn { get; private set; }
        
        /// <summary>
        /// The starting player of the current bidding process
        /// </summary>
        private PlayerPosition startPosition;

        /// <summary>
        /// Indicates whether the current bidding process is complete or not
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// The winner of the bidding in case of the bids are closed, else returns null
        /// </summary>
        public Bid Winner
        {
            get
            {
                if (IsClosed)
                {
                    Bid winner = GetLastNonPassBid();

                    return winner;
                }
                else
                {
                    return null;
                }
            }
        }

        public Bidding(GameSession gameSession,PlayerPosition startPosition)
        {
            this.gameSession = gameSession;

            Bids = new List<Bid>();

            this.startPosition = startPosition;
            CurrentTurn = startPosition;
        }

        /// <summary>
        /// Evaluate and place the specified bid. Checks if the last 3 bids are Pass
        /// or a Pass and Double or Re-double is placed, the last bid is the winner and
        /// the bidding is closed.
        /// </summary>
        public void PlaceBid(Bid bid)
        {
            if (!IsClosed)
            {
                if (gameSession.GetPlayer(CurrentTurn) == bid.Player)
                {
                    //TODO: handle pass&double and re-double bids

                    Bid lastBid = GetLastNonPassBid();
                    if (bid.IsPass || lastBid == null || (lastBid != null && bid.CompareTo(lastBid) > 0))
                    {
                        Bids.Add(bid);
                        if (bid.IsPass && GetNumberOfPassBids() >= 3)
                        {
                            IsClosed = true;
                            gameSession.BiddingComplete();
                        }
                        else
                        {
                            NextTurn();
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The placed bid is less than the last bid");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The player placed a bit not in the right turn");
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot place bids when the bidding is closed");
            }
        }

        private void NextTurn()
        {
            if (!IsClosed)
            {
                if (CurrentTurn == PlayerPosition.West)
                {
                    CurrentTurn = PlayerPosition.South;
                }
                else
                {
                    CurrentTurn++;
                }
            }
        }

        private Bid GetLastNonPassBid()
        {
            for (int i = Bids.Count - 1; i >= 0; i--)
            {
                Bid tempBid = Bids[i];
                if (!tempBid.IsPass)
                {
                    return tempBid;
                }
            }

            return null;
        }
        private int GetNumberOfPassBids()
        {
            int numberOfPassBids = 0;

            for (int i = Bids.Count - 1; i >= 0; i--)
            {
                Bid tempBid = Bids[i];
                if (tempBid.IsPass)
                {
                    numberOfPassBids++;
                }
                else
                {
                    break;
                }
            }
            return numberOfPassBids;
        }
    }
}