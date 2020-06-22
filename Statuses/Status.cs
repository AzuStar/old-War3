using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven.Statuses
{
    public sealed class Status
    {
        public int Id { get; private set; }
        public SimpleStatusType Type { get; private set; }
        public NoxUnit Source { get; private set; }
        public NoxUnit Target { get; private set; }
        timer t;
        effect SpecialEffect;
        /// <summary>
        /// How many stack have been applied, non-stackable has <see cref="Stacking"/> flag set to <see langword="false"/>.
        /// </summary>
        public int Stacks { get; private set; }
        /// <summary>
        /// Limit for stack, eg up to 3/4/5 stacks max
        /// </summary>
        public int StacksLim { get; private set; }
        /// <summary>
        /// Flag
        /// </summary>
        public bool Stacking { get; private set; }
        /// <summary>
        /// Flag
        /// </summary>
        public bool Periodic { get; private set; }
        /// <summary>
        /// <b>Periodic-only</b><br></br>
        /// </summary>
        public float TimeRemain;
        /// <summary>
        /// <b>Periodic-only</b><br></br>
        /// </summary>
        public int PeriodicTicks { get; private set; }
        /// <summary>
        /// Data of a status. Type is dynamic, which means you can make it into list, array...?
        /// </summary>
        public dynamic Data;
        /// <summary>
        /// This is for reseting ability if level greater
        /// </summary>
        public int Level { get; private set; }
        public float TotalDuration { get; private set; }
        public float PeriodicTimeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="level"></param>
        /// <param name="duration"></param>
        /// <param name="stacking"></param>
        /// <param name="periodic"></param>
        internal Status(int id, SimpleStatusType type, NoxUnit source, NoxUnit target, int level, int initialStacks, int stacksLim, float duration, float periodicTimeout, bool stacking, bool periodic)
        {
            //if(type.DataType != null)
            //Data = Activator.CreateInstance(type.DataType);

            Id = id;
            Type = type;
            Source = source;
            Target = target;
            Level = level;
            Stacking = stacking;
            Periodic = periodic;
            PeriodicTimeout = periodicTimeout;
            StacksLim = stacksLim;
            t = CreateTimer();
            if (periodic)
            {
                PeriodicTicks = 0;
                TimeRemain = duration;
                TimerStart(t, PeriodicTimeout, false, PeriodicTimerRestart);
            }
            else
                TimerStart(t, duration, false, Remove);
            if (stacking) Stacks += initialStacks;
            if (Type.Apply != null)
                Type.Apply.Invoke(this);
            SpecialEffect = AddSpecialEffectTarget(Type.Effectpath, target.UnitRef, Type.Attachment);
        }

        internal Status() { }

        private void PeriodicTimerRestart()
        {
            if (Type.Apply != null)
                Type.Apply.Invoke(this);
            PeriodicTicks++;
            TotalDuration += PeriodicTimeout;
            TimeRemain -= PeriodicTimeout;
            if (TimeRemain > 0)
                TimerStart(t, PeriodicTimeout, false, PeriodicTimerRestart);
            else
                Remove();
        }

        public void AddStacks(int stacks)
        {
            if (!Periodic)
                Reset(stacks, Level);
            else Stacks += stacks;
        }
        /// <summary>
        /// Reapplies status, refreshing timer and other stuff. Can't change Status signature.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="stacking"></param>
        /// <param name="periodic"></param>
        public Status Reapply(float duration, int level, int bonusStacks)
        {
            if (Periodic)
            {
                Level = level;
                if (TimeRemain < duration)
                    TimeRemain = duration;
                if (Stacking) Stacks += bonusStacks; // periodic events are never reset
            }
            else
            {
                if (TimerGetRemaining(t) < duration)
                {
                    PauseTimer(t);
                    TotalDuration += TimerGetElapsed(t);
                    TimerStart(t, duration, false, Remove);
                }
                if (Stacking) Reset(bonusStacks, level); // reset if stack
                else if (Level < level) Reset(0, level); // reset to new level
            }
            return this;
        }

        /// <summary>
        /// Kill status completely and cleanup
        /// </summary>
        public void Remove()
        {
            PauseTimer(t);
            TotalDuration += TimerGetElapsed(t);
            if (Type.Reset != null)
                Type.Reset.Invoke(this);
            if (Type.OnRemove != null)
                Type.OnRemove.Invoke(this);
            Target.RemoveStatus(Id);
            DestroyTimer(t);
            DestroyEffect(SpecialEffect);
            t = null;
            Data = null;
            SpecialEffect = null;
            Type = null;
            Source = null;
            Target = null;
        }

        /// <summary>
        /// Reset status applied effect with new stack
        /// </summary>
        public void Reset(int addToStack, int newLevel)
        {
            if (Type.Reset != null)
                Type.Reset.Invoke(this);
            Stacks += addToStack;
            if (StacksLim != 0)
                if (Stacks > StacksLim)
                    Stacks = StacksLim;
            Level = newLevel;
            if (Type.Apply != null)
                Type.Apply.Invoke(this);
        }

    }
}
