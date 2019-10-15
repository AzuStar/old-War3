using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;

namespace War3.NoxRaven
{
    public static class Utils
    {
        public const float PI = 3.14159f;
        public const float DEGTORAD = PI / 180;
        /// <summary>
        /// Display message to every player.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timespan"></param>
        public static void DisplayMessageToEveryone(string msg, float timespan)
        {
            foreach (Player p in Player.AllPlayers)
                DisplayTimedTextToPlayer(p.PlayerRef, 0, 0, timespan, msg);
        }
        /// <summary>
        /// Use this function to invoke something (anything) with a delay.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="effect"></param>
        public static void DelayedInvoke(float timeout, Action effect)
        {
            timer t = CreateTimer();
            TimerStart(t, timeout, false, () => { effect.Invoke(); DestroyTimer(t); });
            t = null;
        }
        public static string NotateNumber(int i)
        {
            string proxy = I2S(i);
            int len = StringLength(proxy);
            if (i >= 1000000000)
                return SubString(proxy, 0, len - 9) + "." + SubString(proxy, len - 9, len - 8) + "B";
            else if(i >= 1000000)
                return SubString(proxy, 0, len - 6) + "." + SubString(proxy, len - 9, len - 8) + "M";
            else if (i >= 1000)
                return SubString(proxy, 0, len - 3) + "." + SubString(proxy, len - 3, len - 2) + "K";
            return proxy;
        }
    }
}
