using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;
using System.Numerics;
using System.Linq;
using NoxRaven.Units;

namespace NoxRaven
{
    public static class Utils
    {
        public delegate bool UnitFilter(NoxUnit nu);
        public const float ROUND_DOWN_CONST_OVERHEAD = 0.19f;
        public static item s_walkableItem;
        public static float s_walkableOverhead = 10;
        public static List<string> s_debugMessages = new List<string>();

        static Utils()
        {
            s_walkableItem = CreateItem(FourCC("afac"), 0, 0); // fix 0,0 bug
            SetItemVisible(s_walkableItem, false);
        }

        /// <summary>
        /// This is a function, that records debug messages
        /// </summary>
        public static void Debug(string str)
        {
            s_debugMessages.Add(str);
            DisplayMessageToEveryone("Debug: " + str, 999999);
        }

        /// <summary>
        /// Display message to every player.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timespan"></param>
        public static void DisplayMessageToEveryone(string msg, float timespan)
        {
            foreach (NoxPlayer p in NoxPlayer.players.Values)
                DisplayTimedTextToPlayer(p._self_, 0, 0, timespan, msg);
        }
        internal static void Error(string message, Type t)
        {
            Master.s_badLoad = true;
            Master.s_errCount++;
            foreach (NoxPlayer p in NoxPlayer.players.Values)
                DisplayTimedTextToPlayer(p._self_, 0, 0, 900f, "|cffFF0000ERROR IN: " + t.FullName + "|r\nMessage: " + message);
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

        // public static void InvokeOverTime(float timeout, float tick, Action effect)
        // {
        //     timer t = CreateTimer();
        //     TimerStart(t, timeout, true, () => { effect.Invoke(); DestroyTimer(t); });
        // }
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
        public static void TextDirectionRandom(string msg, location loc, float size, float r, float g, float b, float alpha, float dur, player p)
        {
            float x = GetLocationX(loc) + GetRandomReal(0, 5) * Cos(GetRandomReal(0, 360) * bj_DEGTORAD);
            float y = GetLocationY(loc) + GetRandomReal(0, 5) * Sin(GetRandomReal(0, 360) * bj_DEGTORAD);
            texttag tt = null;
            if (GetLocalPlayer() == p)
                tt = CreateTextTagLocBJ(msg, loc, 20, size, r, g, b, alpha);
            SetTextTagPos(tt, x, y, 0);
            SetTextTagVelocityBJ(tt, 40, 90);
            SetTextTagPermanent(tt, false);
            SetTextTagFadepoint(tt, dur);
            SetTextTagLifespan(tt, dur + 1);
        }

        public static bool IsCurrentlyWalkable(float x, float y)
        {
            bool flag = false;
            if (IsTerrainPathable(x, y, PATHING_TYPE_WALKABILITY)) return flag;
            SetItemPosition(s_walkableItem, x, y);
            float itemx = GetItemX(s_walkableItem) - x;
            itemx *= itemx;
            float itemy = GetItemY(s_walkableItem) - y;
            itemy *= itemy;
            flag = itemx + itemy <= s_walkableOverhead;
            SetItemVisible(s_walkableItem, false);
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

        /// <summary>
        /// Warcraft 3 just really retarded
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool IsUnitDead(unit u)
        {
            return GetWidgetLife(u) <= 0.305f || IsUnitType(u, UNIT_TYPE_DEAD);
        }

        public static effect DisplayEffect(string effectPath, float x, float y, float duration)
        {
            effect ef = AddSpecialEffect(effectPath, x, y);
            DelayedInvoke(duration, () => { DestroyEffect(ef); });
            return ef;
        }

        public static effect DisplayEffectTarget(string effectPath, string attachmentPoint, widget wi, float duration)
        {
            effect ef = AddSpecialEffectTarget(effectPath, wi, attachmentPoint);
            DelayedInvoke(duration, () => { DestroyEffect(ef); });
            return ef;
        }
        /// <summary>
        /// Returns item's current slot in u's inventory, otherwise -1.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="it"></param>
        /// <returns></returns>
        public static int GetItemSlot(unit u, item it)
        {
            for (int i = 0; i < UnitInventorySize(u); i++)
            {
                if (UnitItemInSlot(u, i) == it) return i;
            }
            return -1;
        }

        public static T RandomItemFromList<T>(List<T> list)
        {
            return list[GetRandomInt(0, list.Count - 1)];
        }

        /// <summary>
        /// Returns list of *amount* units from another list, no repeat. Use filter to REMOVE units you don't want in match.
        /// </summary>
        /// <param name="unitArray"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static List<NoxUnit> RandomNUnitsFromList(NoxUnit[] unitArray, int amount, Predicate<NoxUnit> filter)// can't cast list<> to basetype list<> >_<
        {
            List<NoxUnit> unitList = new List<NoxUnit>(unitArray);
            if (filter != null)
                unitList.RemoveAll(filter);
            if (amount >= unitList.Count) return unitList;
            List<NoxUnit> list = new List<NoxUnit>();
            List<int> range = new List<int>();
            for (int i = 0; i < unitList.Count; i++)
                range.Add(i);
            for (int i = 0; i < amount; i++)
            {
                int index = GetRandomInt(0, range.Count - 1);
                list.Add(unitList[range[index]]);
                range.RemoveAt(index);
            }

            return list;
        }

        public static Vector2 LocationToVector(location loc)
        {
            Vector2 vec = new Vector2(GetLocationX(loc), GetLocationY(loc));
            RemoveLocation(loc);
            return vec;
        }

        public static List<NoxUnit> GetUnitList(UnitFilter filter)
        {
            List<NoxUnit> lis = new List<NoxUnit>();
            foreach (NoxUnit nu in NoxUnit.s_indexer.Values)
                if (filter.Invoke(nu))
                    lis.Add(nu);
            return lis;
        }

        public static string ValusToString<T>(this IEnumerable<T> collection)
        {
            return string.Format("[{0}]", string.Join(", ", collection));
        }

    }
}
