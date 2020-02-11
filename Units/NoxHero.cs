using System;
using System.Collections.Generic;
using System.Text;
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
        public void SetBonusStr(int val)
        {
            BonusStr = val;
            for (int i = Abilities_Strength.Length - 1; i >= 0; i--)
            {
                UnitRemoveAbility(UnitRef, Abilities_Strength[i]);
                int comparator = R2I(Pow(2, i));
                if (comparator <= val)
                {
                    UnitAddAbility(UnitRef, Abilities_Strength[i]);
                    val -= comparator;
                }
            }
        }
        public void SetBonusAgi(int val)
        {
            BonusAgi = val;
            for (int i = Abilities_Agility.Length - 1; i >= 0; i--)
            {
                UnitRemoveAbility(UnitRef, Abilities_Agility[i]);
                int comparator = R2I(Pow(2, i));
                if (comparator <= val)
                {
                    UnitAddAbility(UnitRef, Abilities_Agility[i]);
                    val -= comparator;
                }
            }
        }
        public void SetBonusInt(int val)
        {
            BonusInt = val;
            for (int i = Abilities_Intelligence.Length - 1; i >= 0; i--)
            {
                UnitRemoveAbility(UnitRef, Abilities_Intelligence[i]);
                int comparator = R2I(Pow(2, i));
                if (comparator <= val)
                {
                    UnitAddAbility(UnitRef, Abilities_Intelligence[i]);
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
            SetHeroStr(UnitRef, GetHeroStr(UnitRef, false) + val, true);
        }
        public virtual void AddBaseAgi(int val)
        {
            SetHeroAgi(UnitRef, GetHeroAgi(UnitRef, false) + val, true);
        }
        public virtual void AddBaseInt(int val)
        {
            SetHeroInt(UnitRef, GetHeroInt(UnitRef, false) + val, true);
        }
        /// <summary>
        /// Add experience to the character. Float.
        /// </summary>
        /// <param name="exp"></param>
        public virtual void AddExperience(float exp)
        {
            // For now just use war3 built-in experience manipulator
            int lvl = GetHeroLevel(UnitRef);
            CacheExp += exp * MultiplierExp;
            AddHeroXP(UnitRef, R2I(CacheExp), true);
            CacheExp -= R2I(CacheExp);
            int difference = GetHeroLevel(UnitRef) - lvl;
            if (difference > 0) LevelUp(difference, GetHeroLevel(UnitRef));
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
        public virtual void LevelUp(int times, int previouslvl)
        {
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

        // Leave this empty because heroes are not usually deallocated
        protected override void DeattachClass()
        {
        }

        public NoxHero(Common.unit u) : base(u)
        {
        }

        public new static NoxHero Cast(unit u)
        {
            return (NoxHero)Indexer[GetHandleId(u)];
        }
        public static implicit operator NoxHero(unit u)
        {
            return Cast(u);
        }
    }
}
