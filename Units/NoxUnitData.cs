using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NoxRaven;

using static War3Api.Common;
using NoxRaven.Statuses;
using NoxRaven.UnitAgents;
using NoxRaven.Events;

namespace NoxRaven.Units
{
    /// <summary>
    /// To use in own code simply create class which has inner reference to this class.
    /// Call function <see cref="Master.RunAfterExtensionsReady"/>. It will initialize indexing logic for all units. Put it somewhere after all players and custom classes were initialized.<br><br></br></br>
    /// <b>Never RemoveUnit() indexed unit or it will permaleak.</b>
    /// </summary>
    public partial class NoxUnit
    {
        /// <summary>
        /// Util info.
        /// </summary>
        public bool corpse = false;
        /// <summary>
        /// Unit itself.
        /// </summary>
        public readonly unit _self_;
        private trigger dmgHookTrig;

        private Dictionary<int, Status> _statuses = new Dictionary<int, Status>();

        public Dictionary<int, OnHit> onHits = new Dictionary<int, OnHit>();
        //public Dictionary<int, OnHit> AmHits = new Dictionary<int, OnHit>();

        private Stats _stats = new Stats();
        protected Stats getStats { get => _stats; set => _stats = value; }

        // Actual stats that will be calculated
        public float DMG { get; private set; }
        public float AP { get; private set; }
        public float ARM { get; private set; }
        public float MR { get; private set; }

        private Dictionary<string, BehaviourList<Events.EventArgs>> _events = new Dictionary<string, BehaviourList<Events.EventArgs>>();


        public int ui_baseDMG => R2I(_stats.baseDMG * (1 + _stats.baseDMGPercent));
        public int ui_baseAP => R2I(_stats.baseAP * (1 + _stats.baseAPPercent));
        public int ui_baseARM => R2I(_stats.baseARM * (1 + _stats.baseARMPercent));
        public int ui_baseMR => R2I(_stats.baseMR * (1 + _stats.baseMRPercent));

        public int ui_bonusDMG => R2I(_stats.bonusDMG + _stats.baseDMGPercentBonus * ui_baseDMG + DMG * _stats.totalDMGPercent);
        public int ui_bonusAP => R2I(_stats.bonusAP + _stats.baseAPPercentBonus * ui_baseAP + AP * _stats.totalAPPercent);
        public int ui_bonusARM => R2I(_stats.bonusARM + _stats.baseARMPercentBonus * ui_baseARM + ARM * _stats.totalARMPercent);
        public int ui_bonusMR => R2I(_stats.bonusMR + _stats.baseMRPercentBonus * ui_baseMR + MR * _stats.totalMRPercent);

        #region lookups
        public float lookupBaseDMG => _stats.baseDMG;
        public float lookupBaseDMGPercent => _stats.baseDMGPercent;
        public float lookupBonusDMG => _stats.bonusDMG;
        public float lookupBaseDMGPercentBonus => _stats.baseDMGPercentBonus;
        public float lookupTotalDMGPercent => _stats.totalDMGPercent;
        public float lookupArmorPenetration => _stats.penetrationARM;
        public float lookupAttackSpeed => _stats.attackSpeed;
        public float lookupBaseAttackCooldown => _stats.baseAttackCooldown;
        public float lookupBaseAP => _stats.baseAP;
        public float lookupBaseAPPercent => _stats.baseAPPercent;
        public float lookupBonusAP => _stats.bonusAP;
        public float lookupBaseAPPercentBonus => _stats.baseAPPercentBonus;
        public float lookupTotalAPPercent => _stats.totalAPPercent;
        public float lookupBaseHP => _stats.baseHP;
        public float lookupBaseHPPercent => _stats.baseHPPercent;
        public float lookupBonusHP => _stats.bonusHP;
        public float lookupBaseHPPercentBonus => _stats.baseHPPercentBonus;
        public float lookupTotalHPPercent => _stats.totalHPPercent;
        public float lookupRegenHP => _stats.regenHP;
        public float lookupRegenHPPercent => _stats.regenHPPercent;
        public float lookupBaseMP => _stats.baseMP;
        public float lookupBaseMPPercent => _stats.baseMPPercent;
        public float lookupBonusMP => _stats.bonusMP;
        public float lookupBaseMPPercentBonus => _stats.baseMPPercentBonus;
        public float lookupTotalMPPercent => _stats.totalMPPercent;
        public float lookupRegenMP => _stats.regenMP;
        public float lookupRegenMPPercent => _stats.regenMPPercent;
        public float lookupBaseARM => _stats.baseARM;
        public float lookupBaseARMPercent => _stats.baseARMPercent;
        public float lookupBonusARM => _stats.bonusARM;
        public float lookupBaseARMPercentBonus => _stats.baseARMPercentBonus;
        public float lookupTotalARMPercent => _stats.totalARMPercent;
        public float lookupBaseMR => _stats.baseMR;
        public float lookupBaseMRPercent => _stats.baseMRPercent;
        public float lookupBonusMR => _stats.bonusMR;
        public float lookupBaseMRPercentBonus => _stats.baseMRPercentBonus;
        public float lookupTotalMRPercent => _stats.totalMRPercent;
        public float lookupCritChace => _stats.critChace;
        public float lookupCritDamage => _stats.critDamage;
        public float lookupLifeSteal => _stats.lifeSteal;
        public float lookupSpellVamp => _stats.spellVamp;
        public float lookupIncomingHealing => _stats.incomingHealing;
        public float lookupIncomingMana => _stats.incomingMana;
        public float lookupTriggerChance => _stats.triggerChance;
        public float lookupBaseMSPercent => _stats.baseMSPercent;
        public float lookupBaseMS => _stats.baseMS;
        #endregion

    }
}