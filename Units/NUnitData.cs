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

        private Dictionary<Type, SortedList<Status>> _statuses = new Dictionary<Type, SortedList<Status>>();
        public Dictionary<int, OnHit> onHits = new Dictionary<int, OnHit>();
        // private List<NAbilityInst> abilities = new List<NAbilityInst>();
        private Dictionary<Type, SortedList<NAbility>> abilitiesUniques = new Dictionary<Type, SortedList<NAbility>>();

        public UnitState state = new UnitState();

        private Dictionary<string, BehaviourList<Events.EventArgs>> _events = new Dictionary<string, BehaviourList<Events.EventArgs>>();

        public float AP => state[EUnitState.GREY_AP] + state[EUnitState.GREEN_AP];
        public float ATK => state[EUnitState.GREY_ATK] + state[EUnitState.GREEN_ATK];
        public float ARM => state[EUnitState.GREY_ARM] + state[EUnitState.GREEN_ARM];
        public float RES => state[EUnitState.GREY_RES] + state[EUnitState.GREEN_RES];

        // public int ui_baseDMG => R2I(state.baseDMG * (1 + state.baseDMGPercent));
        // public int ui_baseAP => R2I(state.baseAP * (1 + state.baseAPPercent));
        // public int ui_baseARM => R2I(state.baseARM * (1 + state.baseARMPercent));
        // public int ui_baseMR => R2I(state.baseMR * (1 + state.baseMRPercent));

        // public int ui_bonusDMG => R2I(state.bonusDMG + state.baseDMGPercentBonus * ui_baseDMG + state.DMG * state.totalDMGPercent);
        // public int ui_bonusAP => R2I(state.bonusAP + state.baseAPPercentBonus * ui_baseAP + state.AP * state.totalAPPercent);
        // public int ui_bonusARM => R2I(state.bonusARM + state.baseARMPercentBonus * ui_baseARM + state.ARM * state.totalARMPercent);
        // public int ui_bonusMR => R2I(state.bonusMR + state.baseMRPercentBonus * ui_baseMR + state.MR * state.totalMRPercent);

    }
}