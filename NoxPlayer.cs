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
        public static Dictionary<int, NoxPlayer> AllPlayers = new Dictionary<int, NoxPlayer>();
        /// <summary>
        /// Throw-away value for <see cref="_Self"/>.
        /// </summary>
        public int Id;
        /// <summary>
        /// Real reference to player.
        /// </summary>
        public player _Self;

        public NoxPlayer(int id)
        {
            Id = id;
            _Self = Player(id);
            AllPlayers.Add(id, this);
        }

        public static NoxPlayer GetFromId(int id) => AllPlayers[id];

        public void SendMessage(string msg, float timeout, RecipientType whoGets)
        {
            timer msgt = CreateTimer();
            TimerStart(msgt, timeout, false, () => { BlzDisplayChatMessage(_Self, (int)whoGets, msg); DestroyTimer(msgt); });
        }
    }
}
