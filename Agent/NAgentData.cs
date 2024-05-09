using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Text;
using NoxRaven;
using NoxRaven.Events;
using NoxRaven.UnitAgents;
using static War3Api.Common;

namespace NoxRaven
{
    /// <summary>
    /// To use in own code simply create class which has inner reference to this class.
    /// Call function <see cref="Master.RunAfterExtensionsReady"/>. It will initialize indexing logic for all units. Put it somewhere after all players and custom classes were initialized. Never RemoveUnit() indexed unit or it will permaleak.<br />
    /// Constructor parsed with <see cref="War3Api.Common.unit"/>wc3agent.
    /// </summary>
    public partial class NAgent
    {
        /// <summary>
        /// Util info.
        /// </summary>
        public bool corpse = false;

        /// <summary>
        /// Unit itself.
        /// </summary>
        public readonly unit wc3agent;
        public NPlayer owner => GetOwningPlayer(wc3agent);
        public Vector2 position => new Vector2(GetUnitX(wc3agent), GetUnitY(wc3agent));
        private trigger _dmgHookTrig;
        private trigger _attackHookTrig;
        private trigger _spellEffectTrig;

        public Dictionary<int, OnHit> onHits = new Dictionary<int, OnHit>();
        private Dictionary<Type, SortedList<NAbility>> _abilities =
            new Dictionary<Type, SortedList<NAbility>>();
        private List<UnitDisposable> _disposables = new List<UnitDisposable>();

        public UnitState state = new UnitState();

        private Dictionary<string, BehaviourList<Events.EventArgs>> _events =
            new Dictionary<string, BehaviourList<Events.EventArgs>>();
    }
}
