using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;

namespace War3.NoxRaven
{
    /// <summary>
    /// Class for player data, might add some neat stuff here later on.
    /// Extend to your needs for players in map.
    /// </summary>
    public class Player
    {
        public static List<Player> AllPlayers = new List<Player>();
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
            AllPlayers.Add(this);
        }

        public void SendMessage(string msg, float timeout, RecipientType whoGets)
        {
            timer msgt = CreateTimer();
            TimerStart(msgt, timeout, false, () => { BlzDisplayChatMessage(PlayerRef, (int)whoGets, msg); DestroyTimer(msgt); });
        }
    }
}
