using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven.Events;
using War3Api;
using static War3Api.Common;
namespace NoxRaven.Units
{
    public class NoxHero : NoxUnit
    {
        internal static int[] ABIL_INT = new int[18];
        internal static int[] ABIL_STR = new int[18];
        internal static int[] ABIL_AGI = new int[18];

        protected float cachedStr;
        protected float cachedAgi;
        protected float cachedInt;

        protected float MultiplierExp = 1;
        protected float CacheExp;

        public Action<OnLevelUp> OnLevelUp = (e) => { };

        public static Action<RegenerationTickEvent> HeroRegeneration = (e) =>
        {
            // e.HealthValue += GetHeroStr(e.EventInfo.Target, true) * 0.04f;
            // e.ManaValue += GetHeroInt(e.EventInfo.Target, true) * 0.03f;
        };

        // Compatibility functions
        /// <summary>
        /// Add experience to the character. Float.
        /// </summary>
        /// <param name="exp"></param>
        public virtual void AddExperience(float exp)
        {
            // For now just use war3 built-in experience manipulator
            int lvl = GetHeroLevel(_self_);
            CacheExp += exp * MultiplierExp;
            AddHeroXP(_self_, R2I(CacheExp), true);
            CacheExp -= R2I(CacheExp);
            int difference = GetHeroLevel(_self_) - lvl;
            // if (difference > 0) 
            LevelUp(difference, GetHeroLevel(_self_));
        }
        /// <summary>
        /// Returns how much experience is retured forLevel;
        /// This method, if not overriden, returns War3 default exp required.
        /// </summary>
        /// <param name="forLevel"></param>
        /// <returns></returns>
        public virtual int GetExperienceRequired(int forLevel)
        {
            return R2I(50 * (forLevel * forLevel + forLevel * 3)); // this might need to be used for _absolute_ customization
                                                                   // formula has to match settings in game constant exactly
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
            OnLevelUp(parsEvent);
        }

        public NoxHero(Common.unit u) : base(u)
        {
            // OnRegenerationTick += HeroRegeneration;
        }

        public new static NoxHero Cast(unit u)
        {
            return (NoxHero)Indexer[Common.GetHandleId(u)];
        }
        public static implicit operator NoxHero(unit u)
        {
            return Cast(u);
        }
    }
}