using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven
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
            foreach (NoxPlayer p in NoxPlayer.AllPlayers)
                DisplayTimedTextToPlayer(p.PlayerRef, 0, 0, timespan, msg);
        }
        internal static void Error(string message, Type t)
        {
            Master.BadLoad = true;
            Master.ErrorCount++;
            foreach (NoxPlayer p in NoxPlayer.AllPlayers)
                DisplayTimedTextToPlayer(p.PlayerRef, 0, 0, 90f, "|cffFF0000ERROR IN: " + t.FullName + "|r\nMessage:" + message);
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
            else if (i >= 1000000)
                return SubString(proxy, 0, len - 6) + "." + SubString(proxy, len - 9, len - 8) + "M";
            return proxy;
        }
        public static void RandomDirectedFloatText(string msg, location loc, float size, float r, float g, float b, float alpha, float dur)
        {
            texttag tt = CreateTextTagLocBJ(msg, loc, 0, size, r, g, b, alpha);
            SetTextTagVelocityBJ(tt, 40, 90 + GetRandomReal(-13, 13));
            SetTextTagPermanent(tt, false);
            SetTextTagFadepoint(tt, dur);
            SetTextTagLifespan(tt, dur+1);
            tt = null;
        }
    }
}
