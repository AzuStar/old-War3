using System;
using System.Collections.Generic;
using NoxRaven.Events;
using NoxRaven.Units;

namespace NoxRaven.UnitAgents
{
    /// <summary>
    /// Base class for all abilities. <br />
    /// Constructor parsed with <see cref="NUnit"/>owner. <br />
    /// </summary>
    public abstract class NAbility
    {
        public abstract void OnRemoved();
        public readonly NUnit unit;
        // Some data will be added by inheriting class
        public int level {get; private set;}
        public readonly bool unique = false;

         ///<summary>
        /// Behaviours invoked by the owner of the ability
        ///</summary>
        public readonly List<IBehaviour> localBehaviours = new List<IBehaviour>();
        ///<summary>
        /// Behaviours globally invoked by everything
        ///</summary>
        public readonly List<IBehaviour> globalBehaviours = new List<IBehaviour>();

        ///<summary>
        /// Modefier that will be attached with this ability
        ///</summary>
        public IModifier mod = null;

        public void SetLevel(int lvl){
            level = lvl;

        }

        public NAbility(NUnit unit, bool unique) {
            this.unit = unit;
            this.unique = unique;
        }
    }
}