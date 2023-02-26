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
    /// Call function <see cref="Master.RunAfterExtensionsReady"/>. It will initialize indexing logic for all units. Put it somewhere after all players and custom classes were initialized. Never RemoveUnit() indexed unit or it will permaleak.<br />
    /// Constructor parsed with <see cref="War3Api.Common.unit"/>wc3agent.
    /// </summary>
    public partial class NUnit
    {
        /// <summary>
        /// Util info.
        /// </summary>
        public bool corpse = false;
        /// <summary>
        /// Unit itself.
        /// </summary>
        public readonly unit wc3agent;
        private trigger dmgHookTrig;
        private trigger attackHookTrig;

        private Dictionary<Type, SortedSet<Status>> _statuses = new Dictionary<Type, SortedSet<Status>>();
        public Dictionary<int, OnHit> onHits = new Dictionary<int, OnHit>();
        private List<NAbility> abilities = new List<NAbility>();

        private Stats _stats = new Stats();
        public Stats getStats { get => _stats; set => _stats = value; }

        // Final unit state
        public State state = new State();

        private Dictionary<string, BehaviourList<Events.EventArgs>> _events = new Dictionary<string, BehaviourList<Events.EventArgs>>();


        public int ui_baseDMG => R2I(_stats.baseDMG * (1 + _stats.baseDMGPercent));
        public int ui_baseAP => R2I(_stats.baseAP * (1 + _stats.baseAPPercent));
        public int ui_baseARM => R2I(_stats.baseARM * (1 + _stats.baseARMPercent));
        public int ui_baseMR => R2I(_stats.baseMR * (1 + _stats.baseMRPercent));

        public int ui_bonusDMG => R2I(_stats.bonusDMG + _stats.baseDMGPercentBonus * ui_baseDMG + state.DMG * _stats.totalDMGPercent);
        public int ui_bonusAP => R2I(_stats.bonusAP + _stats.baseAPPercentBonus * ui_baseAP + state.AP * _stats.totalAPPercent);
        public int ui_bonusARM => R2I(_stats.bonusARM + _stats.baseARMPercentBonus * ui_baseARM + state.ARM * _stats.totalARMPercent);
        public int ui_bonusMR => R2I(_stats.bonusMR + _stats.baseMRPercentBonus * ui_baseMR + state.MR * _stats.totalMRPercent);

    }
}