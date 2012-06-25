using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tarneeb.Models
{
    public class BiddingState
    {
        public List<Bid> Bids { get; set; }
        public PlayerPosition CurrentTurn { get; set; }
        public bool IsClosed { get; set; }
        public PlayerPosition? Winner { get; set; }
        public Bid WinnerBid { get; set; }
    }
}
