using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven
{
    public class Status
    {
        public int Id;
        public StatusType Type;
        public UnitEntity Source;
        public UnitEntity Target;
        timer t;
        /// <summary>
        /// How many stack have been applied, stacking begins from 1, 0 means status is non-stackable, flag Staking will also be <see langword="false"/>.
        /// </summary>
        public int Stacks;
        /// <summary>
        /// Flag
        /// </summary>
        public bool Stacking;
        /// <summary>
        /// Flag
        /// </summary>
        public bool Periodic;
        /// <summary>
        /// This is only used by periodic
        /// </summary>
        public float TimeRemain;
        public effect SpecialEffect;
        /// <summary>
        /// Data of a status. Type is dynamic, which means you can make it into list, array...?
        /// </summary>
        public dynamic Data;
        /// <summary>
        /// This is for reseting ability if level greater
        /// </summary>
        public int Level;
        public float TotalDuration;

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
        internal Status(int id, StatusType type, UnitEntity source, UnitEntity target, int level, float duration, bool stacking, bool periodic)
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
            t = CreateTimer();
            if (periodic)
            {
                TimeRemain = duration;
                TimerStart(t, 1f, false, PeriodicTimerRestart);
            }
            else
                TimerStart(t, duration, false, Remove);
            if (stacking) Stacks++;
            if (Type.Apply != null)
                Type.Apply.Invoke(this);
            SpecialEffect = AddSpecialEffectTarget(Type.Effectpath, target.UnitRef, Type.Attachment);
        }

        //internal Status() { }

        private void PeriodicTimerRestart()
        {
            if (Type.Apply != null)
                Type.Apply.Invoke(this);
            TotalDuration += 1;
            TimeRemain--; //later
            if (TimeRemain > 0)
                TimerStart(t, 1f, false, PeriodicTimerRestart);
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
        /// Reapplies status, refreshing timer and other stuff. Can change status data runtime (stacking rules and peridoic)
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="stacking"></param>
        /// <param name="periodic"></param>
        public Status Reapply(float duration, int level, bool stacking, bool periodic)
        {
            if (periodic)
            {
                if (TimeRemain < duration)
                    TimeRemain = duration;
                PauseTimer(t);
                TotalDuration += TimerGetElapsed(t);
                TimerStart(t, 1f, false, PeriodicTimerRestart);
                if (stacking) Stacks++; // periodic events are never reset
            }
            else
            {
                PauseTimer(t);
                TotalDuration += TimerGetElapsed(t);
                TimerStart(t, duration, false, Remove);
                if (stacking) Reset(1, level); // reset if stack
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
            Level = newLevel;
            if (Type.Apply != null)
                Type.Apply.Invoke(this);
        }

    }
}
