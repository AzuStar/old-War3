using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven
{
    public static class Utils
    {
        public static item WalkableItem;
        public static float WalkableOverhead = 10;

        static Utils()
        {
            WalkableItem = CreateItem(FourCC("afac"), 0, 0);
            SetItemVisible(WalkableItem, false);
        }

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
            location newloc = PolarProjectionBJ(loc, GetRandomReal(0, 5), GetRandomReal(0, 360));
            texttag tt = CreateTextTagLocBJ(msg, newloc, 0, size, r, g, b, alpha);
            SetTextTagVelocityBJ(tt, 40, 90);
            SetTextTagPermanent(tt, false);
            SetTextTagFadepoint(tt, dur);
            SetTextTagLifespan(tt, dur + 1);
            RemoveLocation(newloc);
        }

        public static bool IsCurrentlyWalkable(float x, float y)
        {
            bool flag = false;
            if (IsTerrainPathable(x, y, PATHING_TYPE_WALKABILITY)) return flag;
            SetItemPosition(WalkableItem, x, y);
            float itemx = GetItemX(WalkableItem)-x;
            itemx *= itemx;
            float itemy = GetItemY(WalkableItem)-y;
            itemy *= itemy;
            flag = itemx + itemy <= WalkableOverhead;
            SetItemVisible(WalkableItem, false);
            return flag;
        }

        public static float DistanceBetweenPoints(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return SquareRoot(dx * dx + dy * dy);
        }

        public static float AngleBetweenPoints(float x1, float y1, float x2, float y2)
        {
            return Atan2(y2 - y1, x2 - x1) * bj_RADTODEG + 180;
        }

        public static bool IsUnitDead(UnitEntity unit)
        {
            return IsUnitType(unit, UNIT_TYPE_DEAD) || GetWidgetLife(unit.UnitRef) <= 0;
        }

        public static void DisplayEffect(string effectPath, float x, float y, float duration)
        {
            effect ef = AddSpecialEffect(effectPath, x, y);
            DelayedInvoke(duration, ()=> { DestroyEffect(ef); });
        }
    }
}
