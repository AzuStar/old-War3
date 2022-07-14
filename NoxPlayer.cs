using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;

namespace NoxRaven
{
    /// <summary>
    /// Class for player data, might add some neat stuff here later on.
    /// Extend to your needs for players in map.
    /// </summary>
    public class NoxPlayer
    {
        public static Dictionary<int, NoxPlayer> players = new Dictionary<int, NoxPlayer>();
        /// <summary>
        /// Throw-away value for <see cref="_self_"/>.
        /// </summary>
        public int id;
        /// <summary>
        /// Real reference to player.
        /// </summary>
        public player _self_;

        public NoxPlayer(int id)
        {
            this.id = id;
            _self_ = Player(id);
            players.Add(id, this);
        }

        public static NoxPlayer FromId(int id) => players[id];

        public void SendMessage(string msg, float timeout, RecipientType whoGets)
        {
            timer msgt = CreateTimer();
            TimerStart(msgt, timeout, false, () => { BlzDisplayChatMessage(_self_, (int)whoGets, msg); DestroyTimer(msgt); });
        }
    }
}
