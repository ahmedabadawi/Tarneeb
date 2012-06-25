using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarneeb.Models
{
    /// <summary>
    /// Represents a team of two players, either North-South or East-West
    /// </summary>
    public class Team
    {
        /// <summary>
        /// First player of the team; North or East
        /// </summary>
        public Player Player1 { get; set; }
        /// <summary>
        /// Second player of the team; South or West
        /// </summary>
        public Player Player2 { get; set; }

        /// <summary>
        /// The position of the team either NorthSouth or EastWest
        /// </summary>
        public TeamPosition Position { get; set; }

        /// <summary>
        /// Indicates whether the team is ready and has two players or not
        /// </summary>
        public bool IsReady
        {
            get
            {
                return (Player1 != null && Player2 != null);
            }
        }

        public Team()
        {
            Player1 = null;
            Player2 = null;
        }

        /// <summary>
        /// Checks if the specified player is member of the team
        /// </summary>
        public bool IsMember(Player player)
        {
            return (Player1 == player || Player2 == player);
        }
        
        /// <summary>
        /// Make the specified player join the team on the specified position
        /// </summary>
        public void Join(Player player, PlayerPosition position)
        {
            if (
                ((position == PlayerPosition.North ||  position == PlayerPosition.East) && Player1 != null) ||
                ((position == PlayerPosition.South ||  position == PlayerPosition.West) && Player2 != null))
            {

                throw new InvalidOperationException("Another player already holding this position");
            }

            switch (position)
            {
                case PlayerPosition.North:
                case PlayerPosition.East: 
                    Player1 = player;
                    break;
                case PlayerPosition.South:
                case PlayerPosition.West: 
                    Player2 = player;
                    break;
            }
        }

        /// <summary>
        /// Make the specified player leave the team
        /// </summary>
        /// <param name="player"></param>
        public void Leave(Player player)
        {
            if (player == Player1)
            {
                Player1 = null;
            }
            else
            {
                Player2 = null;
            }
        }

        public override string ToString()
        {
            return Position.ToString();
        }
    }

    public enum TeamPosition
    {
        NorthSouth,
        EastWest
    }
}