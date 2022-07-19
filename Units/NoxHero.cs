using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven.Events;
using NoxRaven.UnitAgents;
using War3Api;
using static War3Api.Common;
namespace NoxRaven.Units
{
    public class NoxHero : NoxUnit
    {
        private HeroStats _statsPerLevel = new HeroStats();
        protected HeroStats getStatsPerLevel { get => _statsPerLevel; set => _statsPerLevel = value; }
        protected new HeroStats getStats { get => base.getStats as HeroStats; set => base.getStats = value; }

        protected float CacheExp;

        // Compatibility functions
        /// <summary>
        /// Add experience to the character. Float.
        /// </summary>
        /// <param name="exp"></param>
        public virtual void AddExperience(float exp)
        {
            // For now just use war3 built-in experience manipulator
            int lvl = GetHeroLevel(_self_);
            CacheExp += exp * (1 + getStats.experienceGain);
            AddHeroXP(_self_, R2I(CacheExp), true);
            CacheExp -= R2I(CacheExp);
            int difference = GetHeroLevel(_self_) - lvl;
            if (difference > 0)
                LevelUp(difference, GetHeroLevel(_self_));
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
                caller = _self_,
                previousLevel = previouslvl,
                newLevel = previouslvl + times
            };
            TriggerEvent(parsEvent);
            getStats = (HeroStats)(getStats + getStatsPerLevel * times);
        }

        protected internal NoxHero(Common.unit u, HeroStats statsPerLevel, HeroStats initialStats) : base(u, initialStats)
        {
            if (statsPerLevel != null)
                _statsPerLevel = statsPerLevel;
        }

        public new static NoxHero Cast(unit u)
        {
            return (NoxHero)s_indexer[Common.GetHandleId(u)];
        }
        public static implicit operator NoxHero(unit u)
        {
            return Cast(u);
        }
    }
}