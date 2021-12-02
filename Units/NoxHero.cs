using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven.Events.EventTypes;
using NoxRaven.Events.Metas;
using War3Api;
using static War3Api.Common;
namespace NoxRaven.Units
{
    public class NoxHero : NoxUnit
    {
        internal static int[] Abilities_Intelligence = new int[18];
        internal static int[] Abilities_Strength = new int[18];
        internal static int[] Abilities_Agility = new int[18];
        // +xx stat
        protected int BonusStr;
        protected int BonusAgi;
        protected int BonusInt;

        protected float CacheStr;
        protected float CacheAgi;
        protected float CacheInt;

        protected float PerLevelStr;
        protected float PerLevelAgi;
        protected float PerLevelInt;

        protected float MultiplierExp = 1;
        protected float CacheExp;

        // Access functions
        public int GetBonusStr() => BonusStr;
        public int GetBonusAgi() => BonusAgi;
        public int GetBonusInt() => BonusInt;

        public Action<LevelUpEvent> OnLevelUp = (e) => { };

        public static Action<RegenerationTickEvent> HeroRegeneration = (e) =>
        {
            e.HealthValue += GetHeroStr(e.EventInfo.Target, true) * 0.04f;
            e.ManaValue += GetHeroInt(e.EventInfo.Target, true) * 0.03f;
        };

        public void SetBonusStr(int val)
        {
            BonusStr = val;
            for (int i = Abilities_Strength.Length - 1; i >= 0; i--)
            {
                UnitRemoveAbility(_Self, Abilities_Strength[i]);
                int comparator = R2I(Pow(2, i));
                if (comparator <= val)
                {
                    UnitAddAbility(_Self, Abilities_Strength[i]);
                    val -= comparator;
                }
            }
        }
        public void SetBonusAgi(int val)
        {
            BonusAgi = val;
            for (int i = Abilities_Agility.Length - 1; i >= 0; i--)
            {
                UnitRemoveAbility(_Self, Abilities_Agility[i]);
                int comparator = R2I(Pow(2, i));
                if (comparator <= val)
                {
                    UnitAddAbility(_Self, Abilities_Agility[i]);
                    val -= comparator;
                }
            }
        }
        public void SetBonusInt(int val)
        {
            BonusInt = val;
            for (int i = Abilities_Intelligence.Length - 1; i >= 0; i--)
            {
                UnitRemoveAbility(_Self, Abilities_Intelligence[i]);
                int comparator = R2I(Pow(2, i));
                if (comparator <= val)
                {
                    UnitAddAbility(_Self, Abilities_Intelligence[i]);
                    val -= comparator;
                }
            }
        }
        // Compatibility functions
        public virtual void AddBonusStr(int val)
        {
            SetBonusStr(BonusStr + val);
        }
        public virtual void AddBonusAgi(int val)
        {
            SetBonusAgi(BonusAgi + val);
        }
        public virtual void AddBonusInt(int val)
        {
            SetBonusInt(BonusInt + val);
        }
        public virtual void AddBaseStr(int val)
        {
            SetHeroStr(_Self, GetHeroStr(_Self, false) + val, true);
        }
        public virtual void AddBaseAgi(int val)
        {
            SetHeroAgi(_Self, GetHeroAgi(_Self, false) + val, true);
        }
        public virtual void AddBaseInt(int val)
        {
            SetHeroInt(_Self, GetHeroInt(_Self, false) + val, true);
        }
        /// <summary>
        /// Add experience to the character. Float.
        /// </summary>
        /// <param name="exp"></param>
        public virtual void AddExperience(float exp)
        {
            // For now just use war3 built-in experience manipulator
            int lvl = GetHeroLevel(_Self);
            CacheExp += exp * MultiplierExp;
            AddHeroXP(_Self, R2I(CacheExp), true);
            CacheExp -= R2I(CacheExp);
            int difference = GetHeroLevel(_Self) - lvl;
            // if (difference > 0) 
            LevelUp(difference, GetHeroLevel(_Self));
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
            LevelUpEvent parsEvent = new LevelUpEvent()
            {
                EventInfo = new LevelUpMeta()
                {
                    Hero = _Self,
                    PreviousLevel = previouslvl,
                    NewLevel = previouslvl + times
                }
            };
            OnLevelUp(parsEvent);
            CacheStr += PerLevelStr * times;
            CacheAgi += PerLevelAgi * times;
            CacheInt += PerLevelInt * times;
            int complete = R2I(CacheStr);
            CacheStr -= complete;
            AddBaseStr(complete);
            complete = R2I(CacheAgi);
            CacheAgi -= complete;
            AddBaseAgi(complete);
            complete = R2I(CacheInt);
            CacheInt -= complete;
            AddBaseInt(complete);
        }

        public NoxHero(Common.unit u) : base(u)
        {
            OnRegenerationTick += HeroRegeneration;
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