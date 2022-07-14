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
        private trigger DamageTrig;

        private Dictionary<int, Status> Statuses = new Dictionary<int, Status>();

        public Dictionary<int, OnHit> OnHits = new Dictionary<int, OnHit>();
        //public Dictionary<int, OnHit> AmHits = new Dictionary<int, OnHit>();

        private Stats _stats = new Stats();

        // Actual stats that will be calculated
        public float DMG { get; private set; }
        public float AP { get; private set; }
        public float ARM { get; private set; }
        public float MR { get; private set; }

        private Dictionary<string, BehaviourList<Events.EventArgs>> _events = new Dictionary<string, BehaviourList<Events.EventArgs>>();

    }
}