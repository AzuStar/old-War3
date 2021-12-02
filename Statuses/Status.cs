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

        public enum Polarity : int
        {
            Neutral, Positive, Negative, Reserved
        }
        public int Id { get; private set; }
        public TimedType Type { get; private set; }
        public NoxUnit Source { get; private set; }
        public NoxUnit Target { get; private set; }
        timer t;
        effect SpecialEffect;
        /// <summary>
        /// How many stack have been applied, non-stackable has <see cref="Stacking"/> flag set to <see langword="false"/>.
        /// </summary>
        public int Stacks { get; private set; } = 0;
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
        ///// <summary>
        ///// Flag that specifies if effect is permanent
        ///// </summary>
        //public bool Permanent { get; private set; }
        public float Duration { get; private set; }
        /// <summary>
        /// <b>Periodic-only</b><br></br>
        /// </summary>
        public float TimeRemain { get; private set; }
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
        public float TimeElapsed { get; private set; }
        public float PeriodicTimeout { get; private set; }

        /// <summary>
        /// Permanent status.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="level"></param>
        /// <param name="duration"></param>
        /// <param name="stacking"></param>
        /// <param name="periodic"></param>
        private Status(int id, TimedType type, NoxUnit source, NoxUnit target, int level, int initialStacks, int stacksLim, float duration, float periodicTimeout, bool stacking, bool periodic, bool permanent = false)
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
            //Permanent = permanent;
            PeriodicTimeout = periodicTimeout;
            StacksLim = stacksLim;
            Duration = duration;
            t = CreateTimer();
            if (periodic)
            {
                PeriodicTicks = 0;
                TimeRemain = duration;
                TimerStart(t, PeriodicTimeout, false, PeriodicTimerRestart);
            }
            else
            {
                TimerStart(t, duration, false, Remove);
            }
            if (stacking) Stacks = initialStacks;
            if (Type.Apply != null)
                Type.Apply.Invoke(this);
            if (Type.Effectpath != null && Type.Attachment != null)
                SpecialEffect = AddSpecialEffectTarget(Type.Effectpath, target._Self, Type.Attachment);
        }

        ///// <summary>
        ///// Permanent Status
        ///// </summary>
        //internal Status(int id, StatusType type, NoxUnit source, NoxUnit target, int level)
        //    : this(id, type, source, target, level, 0, 0, 0, 0, false, false, true) { }

        ///// <summary>
        ///// Permanent Stacking
        ///// </summary>
        //internal Status(int id, StatusType type, NoxUnit source, NoxUnit target, int level, int initialStacks, int stacksLim)
        //    : this(id, type, source, target, level, initialStacks, stacksLim, 0, 0, true, false, true) { }

        ///// <summary>
        ///// Permanent Periodic
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="type"></param>
        ///// <param name="source"></param>
        ///// <param name="target"></param>
        ///// <param name="level"></param>
        //internal Status(int id, StatusType type, NoxUnit source, NoxUnit target, int level)
        //    : this(id, type, source, target, level, 0, 0, 0, 0, false, true, true) { }

        /// <summary>
        /// Timed Status
        /// </summary>
        internal Status(int id, TimedType type, NoxUnit source, NoxUnit target, int level, float duration)
        : this(id, type, source, target, level, 0, 0, duration, 0, false, false) { }

        /// <summary>
        /// Timed Stacking Status
        /// </summary>
        internal Status(int id, TimedType type, NoxUnit source, NoxUnit target, int level, float duration, int initialStacks, int stacksLim)
        : this(id, type, source, target, level, initialStacks, stacksLim, duration, 0, false, false) { }

        /// <summary>
        /// Periodic Timed Status
        /// </summary>
        internal Status(int id, TimedType type, NoxUnit source, NoxUnit target, int level, float duration, float periodicTimeout)
        : this(id, type, source, target, level, 0, 0, duration, periodicTimeout, false, true) { }

        /// <summary>
        /// Periodic Timed Stacking Status
        /// </summary>
        internal Status(int id, TimedType type, NoxUnit source, NoxUnit target, int level, float duration, float peridoticTimeout, int initialStacks, int stacksLim)
        : this(id, type, source, target, level, initialStacks, stacksLim, duration, peridoticTimeout, false, true) { }

        internal Status() { }

        private void PeriodicTimerRestart()
        {
            if (Type.Apply != null)
                Type.Apply.Invoke(this);
            PeriodicTicks++;
            TimeElapsed += PeriodicTimeout;
            TimeRemain -= PeriodicTimeout;
            if (TimeRemain > 0)
                TimerStart(t, PeriodicTimeout, false, PeriodicTimerRestart);
            else
                Remove();
        }

        public void AddStacks(int stacks)
        {
            if (!Periodic)
                Reset(stacks, 0);
            else Stacks += stacks;
        }
        /// <summary>
        /// Reapplies status, refreshing timer and other stuff. Can't change Status signature.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="stacking"></param>
        /// <param name="periodic"></param>
        public Status Reapply(float bonusDuration, int bonusLevel, int bonusStacks)
        {
            int prevLevel = Level;
            Duration += bonusDuration;
            if (Periodic)
            {
                if (TimeRemain < Duration)
                    TimeRemain = Duration;
                if (Stacking) Stacks += bonusStacks; // periodic events are never reset
            }
            else
            {
                if (TimerGetRemaining(t) < Duration)
                {
                    PauseTimer(t);
                    TimeElapsed += TimerGetElapsed(t);
                    TimerStart(t, Duration, false, Remove);
                }
                if (Stacking) Reset(bonusStacks, Level + bonusLevel); // reset if stack
                else if (Level + bonusLevel > Level) Reset(0, bonusLevel); // reset to new level
            }
            return this;
        }

        /// <summary>
        /// Kill status completely and cleanup
        /// </summary>
        public void Remove()
        {
            PauseTimer(t);
            TimeElapsed += TimerGetElapsed(t);
            DestroyTimer(t);
            if (Type.Reset != null)
                Type.Reset.Invoke(this);
            if (Type.OnRemove != null)
                Type.OnRemove.Invoke(this);
            Target.RemoveStatus(Id);
            DestroyEffect(SpecialEffect);
            t = null;
            Data = null;
            SpecialEffect = null;
            Type = null;
            Source = null;
            Target = null;
        }

        /// <summary>
        /// Reset status applied effect with new stack and level
        /// </summary>
        public void Reset(int addToStack, int addToLevel)
        {
            if (Type.Reset != null)
                Type.Reset.Invoke(this);
            Stacks += addToStack;
            if (StacksLim != 0)
                if (Stacks > StacksLim)
                    Stacks = StacksLim;
            Level += addToLevel;
            if (Type.Apply != null)
                Type.Apply.Invoke(this);
        }

    }
}
