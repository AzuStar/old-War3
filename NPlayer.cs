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
    public class NPlayer
    {
        public static Dictionary<int, NPlayer> players = new Dictionary<int, NPlayer>();
        /// <summary>
        /// Throw-away value for <see cref="wc3agent"/>.
        /// </summary>
        public int id;
        /// <summary>
        /// Real reference to player.
        /// </summary>
        public player wc3agent;

        public NPlayer(int id)
        {
            this.id = id;
            wc3agent = Player(id);
            players.Add(id, this);
        }

        public static NPlayer FromId(int id) => players[id];

        public void SendMessage(string msg, float timeout, RecipientType whoGets)
        {
            timer msgt = CreateTimer();
            TimerStart(msgt, timeout, false, () => { BlzDisplayChatMessage(wc3agent, (int)whoGets, msg); DestroyTimer(msgt); });
        }

        public static implicit operator player(NPlayer p) => p.wc3agent;
        public static implicit operator NPlayer(player p) => FromId(GetPlayerId(p));
    }
}
