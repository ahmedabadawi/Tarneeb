using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarneeb.Models
{
    /// <summary>
    /// Represents a player
    /// </summary>
    public class Player
    {
        private const int idleTimeThreshold = 1;    //1 minute of inactivity is considered idle
        /// <summary>
        /// The authentication token
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// The player display name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The time and date of the last operation or pull done by the player. 
        /// </summary>
        public DateTime LastSeen { get; private set; }

        /// <summary>
        /// Indicates whether the player hasn't done any activity for longer
        /// than the threshold 1 minute
        /// </summary>
        public bool IsIdle
        {
            get
            {
                return DateTime.Now.Subtract(LastSeen).TotalMinutes >= idleTimeThreshold;
            }
        }

        public Player(Guid id, string name)
        {
            this.Name = name;
            this.Id = id;
        }

        /// <summary>
        /// Update the last seen property to now
        /// </summary>
        public void UpdateLastSeen()
        {
            LastSeen = DateTime.Now;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object other)
        {
            return this.Id.Equals(((Player)other).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public enum PlayerPosition
    {
        South = 1,
        East,
        North,
        West
    }
}