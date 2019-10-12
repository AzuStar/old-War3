using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;

namespace War3.NoxRaven
{
    /// <summary>
    /// Class for player data, might add some neat stuff here later on.
    /// Use class for real players, or extend to your needs for computer players.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Throw-away value for <see cref="PlayerRef"/>.
        /// </summary>
        public int Id;
        /// <summary>
        /// Real reference to player.
        /// </summary>
        public player PlayerRef;
        public Player(int id)
        {
            Id = id;
            PlayerRef = Player(id);
        }
    }
}
