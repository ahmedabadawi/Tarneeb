using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tarneeb.Models
{
    public class GamePlayState
    {
        public PlayerPosition CurrentTurn { get; set; }
        public Dictionary<TeamPosition,int> TricksWon { get; set; }
        public List<Card> CurrentCards { get; set; }
        public Dictionary<PlayerPosition, Card> CurrentTrick { get; set; }
        public Suit CurrentTrickBaseSuit { get; set; }
    }
}
