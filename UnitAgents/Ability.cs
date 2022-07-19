using System;
using System.Collections.Generic;
using NoxRaven.Events;

namespace NoxRaven.UnitAgents
{
    public abstract class Ability
    {
        // Some data will be added by inheriting class
        public int level;

        ///<summary>
        /// Behaviours invoked by the owner of the ability
        ///</summary>
        protected readonly List<IPriorityBehaviour> localBehaviours = new List<IPriorityBehaviour>();
        ///<summary>
        /// Behaviours globally invoked by everything
        ///</summary>
        protected readonly List<IPriorityBehaviour> globalBehaviours = new List<IPriorityBehaviour>();

        ///<summary>
        /// Modefier that will be attached with this ability
        ///</summary>
        public IModifier mod = null;
        // This list will be used to init all behaviours.
        public List<IPriorityBehaviour> GetLocalBehaviours() => localBehaviours;
        public List<IPriorityBehaviour> GetGlobalBehaviours() => globalBehaviours;


        public Ability() { }
    }
}