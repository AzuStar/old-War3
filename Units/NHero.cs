using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven.Data;
using NoxRaven.Events;
using NoxRaven.UnitAgents;
using War3Api;
using static War3Api.Common;
namespace NoxRaven.Units
{
    public class NHero : NUnit
    {
        public NDataModifier statsPerLevel = new NDataModifier();

        protected float CacheExp;

        // Compatibility functions
        /// <summary>
        /// Add experience to the character. Float.
        /// </summary>
        /// <param name="exp"></param>
        public virtual void AddExperience(float exp)
        {
            // For now just use war3 built-in experience manipulator
            int lvl = GetHeroLevel(wc3agent);
            CacheExp += exp * (1 + state[EUnitState.EXP_RATE]);
            AddHeroXP(wc3agent, R2I(CacheExp), true);
            CacheExp -= R2I(CacheExp);
            int difference = GetHeroLevel(wc3agent) - lvl;
            if (difference > 0)
                LevelUp(difference, GetHeroLevel(wc3agent));
        }
        /// <summary>
        /// This is called every levelup, and even multiple times if gained experience more than level table.
        /// </summary>
        /// <param name="times"></param>
        /// <param name="previouslvl"></param>
        public void LevelUp(int times, int previouslvl)
        {
            OnLevelUp parsEvent = new OnLevelUp()
            {
                caller = wc3agent,
                previousLevel = previouslvl,
                newLevel = previouslvl + times
            };
            TriggerEvent(parsEvent);
            AddModifier(statsPerLevel * times);
        }

        protected internal NHero(Common.unit u, NDataModifier initialStats = null) : base(u, initialStats)
        {
            AddModifier(statsPerLevel * GetHeroLevel(wc3agent));
            SetUnitState(wc3agent, UNIT_STATE_LIFE, 9999999);
            SetUnitState(wc3agent, UNIT_STATE_MANA, 9999999);
        }

        public new static NHero Cast(unit u)
        {
            return (NHero)s_indexer[Common.GetHandleId(u)];
        }
        public static implicit operator NHero(unit u)
        {
            return Cast(u);
        }
    }
}