using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven
{
    /// <summary>
    /// Some really special utilities that do not really belong (or may be not yet) in NoxUnit
    /// </summary>
    public static class UnitExperimentals
    {
        static timer s_knockbackTimer;
        static List<PushedUnit> s_pushed = new List<PushedUnit>();
        static UnitExperimentals()
        {
            s_knockbackTimer = CreateTimer();
            // Max possible smoothness @ 64 fps
            TimerStart(s_knockbackTimer, Master.FPS_TIME, true, () =>
            {
                if (s_pushed.Count > 0)
                    foreach (PushedUnit pu in s_pushed)
                        if (pu.Move())
                            s_pushed.Remove(pu);
            });
        }
        /// <summary>
        /// Pulls targets from range over duration with power stress. <br />
        /// Power - how hard this algortihm tries to pull targets. power = 1 means everyone in range will gets slowly pulled towards center.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="duration"></param>
        /// <param name="power"></param>
        //public static void PullTargets(float range, float duration, float power)
        //{

        //}

        public static void PushTarget(unit target, float duration, float angle, float range, bool actionBlocked)
        {
            s_pushed.Add(new PushedUnit(target, duration, angle, range, actionBlocked));
        }

    }

    public class PushedUnit
    {
        unit Unit;
        public float TimeLeft;
        public float Sin;
        public float Cos;
        public bool ActionsBlocked;

        public PushedUnit(unit Unit, float duration, float angle, float range, bool actionBlocked)
        {
            this.Unit = Unit;
            TimeLeft = duration;
            float push = range / duration * Master.FPS_TIME;
            Sin = push * Sin(angle * bj_DEGTORAD);
            Cos = push * Cos(angle * bj_DEGTORAD);
            ActionsBlocked = actionBlocked;
        }

        public bool Move()
        {
            float xi = GetUnitX(Unit) + Cos;
            float yi = GetUnitY(Unit) + Sin;
            if (Utils.IsCurrentlyWalkable(xi, yi))
                if (ActionsBlocked)
                {
                    SetUnitPosition(Unit, xi, yi);
                }
                else
                {
                    SetUnitX(Unit, xi);
                    SetUnitY(Unit, yi);
                }
            return (TimeLeft -= Master.FPS_TIME) <= 0;
        }
    }
}
