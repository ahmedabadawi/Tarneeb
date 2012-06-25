using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tarneeb.Models
{
    public class GameSessionState
    {
        public Guid Id { get; set; }
        public Dictionary<PlayerPosition,Player> Players { get; set; }
        public GameSessionStatus Status { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
