using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;
using System.Numerics;
using War3Api;

namespace NoxRaven
{
    /// <summary>
    /// Some really special utilities that do not really belong (or may be not yet) in NUnit
    /// </summary>
    public static class UnitExperimentals
    {
        static List<PushedUnit> pushedList = new List<PushedUnit>();

        internal static void InitUnitExperimentals()
        {
            Master.s_globalTick.Add((delta) =>
            {
                if (pushedList.Count > 0)
                    foreach (PushedUnit pu in pushedList)
                        if (pu.Move(delta))
                        {
                            pushedList.Remove(pu);
                        }
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

        public static void Dash(unit target, float duration, Vector2 targetPosition, bool actionBlocked)
        {
            Vector2 currentPosition = new Vector2(GetUnitX(target), GetUnitY(target));
            float range = Maffs.GetDistance(currentPosition, targetPosition);
            float angle = Maffs.GetFacingTowardsAngle(targetPosition, currentPosition);
            PushTarget(target, duration, angle, range, actionBlocked);
        }

        public static void PushTarget(unit target, float duration, float angle, float range, bool actionBlocked)
        {
            pushedList.Add(new PushedUnit(target, duration, angle, range, actionBlocked));
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
            float push = range / duration * Master.TICK_DELTA;
            Sin = push * Sin(angle * bj_DEGTORAD);
            Cos = push * Cos(angle * bj_DEGTORAD);
            ActionsBlocked = actionBlocked;
        }

        public bool Move(float delta)
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
            if (IsUnitDeadBJ(Unit))
            {
                return true;
            }
            return (TimeLeft -= delta) <= 0;
        }
    }
}
