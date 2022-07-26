using System.Collections.Generic;
using System.Linq;

namespace NoxRaven.UnitAgents
{
    ///<summary>
    /// Stats of the NoxUnit <br/>
    /// Start custom stats at key (including) 49
    /// Remineder: heroes use 49 for experience rate
    ///</summary>
    public class Stats
    {
        /*
        // Gets all stats: 
        ^(?:(?!public (?:\w+) (\w+)(?:.+)).)*$\n
        //replace with empty
        
        // replace var names
        ^ *public (?:\w+) (\w+)(?:.+)$
        left.$1-=right.$1;
        

        // Stats to lookups:
        (double|float|int) (\w)(\w+)(?: = .+|;)?
        $1 lookup\U$2$3=>_stats.$2$3;
        */
        public override string ToString()
        {
            return string.Format("[{0}]", string.Join(", ", _statCollection.Values));
        }
        private Dictionary<int, float> _statCollection = new Dictionary<int, float>();
        protected void SetStat(int key, float value)
        {
            if (_statCollection.ContainsKey(key))
                _statCollection[key] = value;
            else
                _statCollection.Add(key, value);
        }
        protected float GetStat(int key)
        {
            if (_statCollection.ContainsKey(key))
                return _statCollection[key];
            return 0;
        }

        // ****************
        // * Physical Dmg *
        // ****************
        public float baseDMG { get => GetStat(0); set => SetStat(0, value); }
        public float baseDMGPercent { get => GetStat(1); set => SetStat(1, value); }
        public float bonusDMG { get => GetStat(2); set => SetStat(2, value); }
        public float baseDMGPercentBonus { get => GetStat(3); set => SetStat(3, value); }
        public float totalDMGPercent { get => GetStat(4); set => SetStat(4, value); }

        /// <summary>
        /// Base attack speed / attackSpeed = New Attack Speed <br />
        /// 1/2 = 0.5 (2 attacks a second)<br />
        /// 2/1 = 2 (0.5 attacks a second)
        /// </summary>
        public float attackSpeed { get => GetStat(7); set => SetStat(7, value); }
        public float baseAttackCooldown { get => GetStat(8); set => SetStat(8, value); }

        // *****************
        // * Ability Power *
        // *****************
        public float baseAP { get => GetStat(9); set => SetStat(9, value); }
        public float baseAPPercent { get => GetStat(10); set => SetStat(10, value); }
        public float bonusAP { get => GetStat(11); set => SetStat(11, value); }
        public float baseAPPercentBonus { get => GetStat(12); set => SetStat(12, value); }
        public float totalAPPercent { get => GetStat(13); set => SetStat(13, value); }


        // **********
        // * Vitals *
        // **********
        public float baseHP { get => GetStat(14); set => SetStat(14, value); }
        public float baseHPPercent { get => GetStat(15); set => SetStat(15, value); }
        public float bonusHP { get => GetStat(16); set => SetStat(16, value); }
        public float baseHPPercentBonus { get => GetStat(17); set => SetStat(17, value); }
        public float totalHPPercent { get => GetStat(18); set => SetStat(18, value); }
        public float regenHP { get => GetStat(19); set => SetStat(19, value); }
        public float regenHPPercent { get => GetStat(20); set => SetStat(20, value); }

        public float baseMP { get => GetStat(21); set => SetStat(21, value); }
        public float baseMPPercent { get => GetStat(22); set => SetStat(22, value); }
        public float bonusMP { get => GetStat(23); set => SetStat(23, value); }
        public float baseMPPercentBonus { get => GetStat(24); set => SetStat(24, value); }
        public float totalMPPercent { get => GetStat(25); set => SetStat(25, value); }
        public float regenMP { get => GetStat(26); set => SetStat(26, value); }
        public float regenMPPercent { get => GetStat(27); set => SetStat(27, value); }

        public float baseARM { get => GetStat(28); set => SetStat(28, value); }
        public float baseARMPercent { get => GetStat(29); set => SetStat(29, value); }
        public float bonusARM { get => GetStat(30); set => SetStat(30, value); }
        public float baseARMPercentBonus { get => GetStat(31); set => SetStat(31, value); }
        public float totalARMPercent { get => GetStat(32); set => SetStat(32, value); }

        public float baseMR { get => GetStat(33); set => SetStat(33, value); }
        public float baseMRPercent { get => GetStat(34); set => SetStat(34, value); }
        public float bonusMR { get => GetStat(35); set => SetStat(35, value); }
        public float baseMRPercentBonus { get => GetStat(36); set => SetStat(36, value); }
        public float totalMRPercent { get => GetStat(37); set => SetStat(37, value); }


        public float penetrationARM { get => GetStat(5); set => SetStat(5, value); }
        public float penetrationMR { get => GetStat(6); set => SetStat(6, value); }
        public float critChace { get => GetStat(38); set => SetStat(38, value); }
        public float critDamage { get => GetStat(39); set => SetStat(39, value); }
        /// <summary>
        /// Life restored from amount of ALL PHYSICAL damage DEALT <br/>
        /// Damage flags affected by life steal: isBaseAttack, isCrit, isOnhit, isPhysical
        /// </summary>
        public float lifeSteal { get => GetStat(40); set => SetStat(40, value); }
        /// <summary>
        /// Life restored from amount of ALL spell damage DEALT <br/>
        /// Damage flags affected by spell vamp: isPhysical, isCrit, isMagical
        /// </summary>
        public float spellVamp { get => GetStat(41); set => SetStat(41, value); }
        /// <summary>
        /// Healing from all sources will be multiplied by this.
        /// </summary>
        public float incomingHealing { get => GetStat(42); set => SetStat(42, value); }
        /// <summary>
        /// When mana is added this is multiplier.
        /// </summary>
        public float incomingMana { get => GetStat(43); set => SetStat(43, value); }

        //*********
        // * Util *
        //*********
        /// <summary>
        /// This is a Chance diceroll event happens. 
        /// For example, passive ability with 25% activation chance to deal extra damage will have 50% chance if this value is 2.
        /// </summary>
        public float triggerChance { get => GetStat(44); set => SetStat(44, value); }
        public float baseMSPercent { get => GetStat(45); set => SetStat(45, value); }
        public float baseMS { get => GetStat(46); set => SetStat(46, value); }
        public float dodgeChance { get => GetStat(47); set => SetStat(47, value); }
        public float blockChance { get => GetStat(48); set => SetStat(48, value); }


        /// <summary>
        /// Returned Stats are new class instance
        /// </summary>
        public static Stats operator +(Stats left, Stats right)
        {
            HashSet<int> keys = new HashSet<int>();
            Stats newstats = new Stats();
            left._statCollection.Keys.Union(right._statCollection.Keys).ToList().ForEach(key => keys.Add(key));
            foreach (int key in keys)
            {
                newstats.SetStat(key, left.GetStat(key) + right.GetStat(key));
            }
            return newstats;
        }
        /// <summary>
        /// Returned Stats are new class instance
        /// </summary>
        public static Stats operator -(Stats left, Stats right)
        {
            HashSet<int> keys = new HashSet<int>();
            Stats newstats = new Stats();
            left._statCollection.Keys.Union(right._statCollection.Keys).ToList().ForEach(key => keys.Add(key));
            foreach (int key in keys)
            {
                newstats.SetStat(key, left.GetStat(key) - right.GetStat(key));
            }
            return newstats;
        }
        /// <summary>
        /// Returned Stats are new class instance
        /// </summary>
        public static Stats operator *(Stats left, Stats right)
        {
            HashSet<int> keys = new HashSet<int>();
            Stats newstats = new Stats();
            left._statCollection.Keys.Union(right._statCollection.Keys).ToList().ForEach(key => keys.Add(key));
            foreach (int key in keys)
            {
                newstats.SetStat(key, left.GetStat(key) * right.GetStat(key));
            }
            return newstats;
        }
        /// <summary>
        /// Returned Stats are new class instance
        /// </summary>
        public static Stats operator /(Stats left, Stats right)
        {
            HashSet<int> keys = new HashSet<int>();
            Stats newstats = new Stats();
            left._statCollection.Keys.Union(right._statCollection.Keys).ToList().ForEach(key => keys.Add(key));
            foreach (int key in keys)
            {
                if (right.GetStat(key) == 0)
                    newstats.SetStat(key, 0);
                else
                    newstats.SetStat(key, left.GetStat(key) / right.GetStat(key));
            }
            return newstats;
        }
        /// <summary>
        /// Returned Stats are new class instance
        /// </summary>
        public static Stats operator *(Stats left, float right)
        {
            Stats newstats = new Stats();
            foreach (int key in left._statCollection.Keys)
            {
                newstats.SetStat(key, left._statCollection[key] * right);
            }
            return newstats;
        }
        /// <summary>
        /// Returned Stats are new class instance
        /// </summary>
        public static Stats operator /(Stats left, float right)
        {
            Stats newstats = new Stats();
            foreach (int key in left._statCollection.Keys)
            {
                newstats.SetStat(key, left._statCollection[key] / right);
            }
            return newstats;
        }
    }
}