using System;
using System.Collections.Generic;
using NoxRaven.Data;
using NoxRaven.Events;
using NoxRaven.Units;

namespace NoxRaven.UnitAgents
{
    /// <summary>
    /// Base class for all abilities. <br />
    /// Constructor parsed with <see cref="NUnit"/>owner. <br />
    /// </summary>
    public abstract class NAbility : IComparable<NAbility>
    {
        // Some data will be added by inheriting class
        public NUnit owner;
        public int level { get; private set; }
        public bool unique { get; private set; }

        protected virtual NDataModifier GetModifier() { return null; }
        protected virtual List<IBehaviour> GetLocalBehaviours() { return new List<IBehaviour>(); }
        protected virtual List<IBehaviour> GetGlobalBehaviours() { return new List<IBehaviour>(); }

        ///<summary>
        /// Behaviours invoked by the owner of the ability
        ///</summary>
        internal List<IBehaviour> localBehaviours;
        ///<summary>
        /// Behaviours globally invoked by everything
        ///</summary>
        internal List<IBehaviour> globalBehaviours;
        ///<summary>
        /// Modefier that will be attached with this ability
        ///</summary>
        internal NDataModifier modifier;

        public void SetLevel(int lvl)
        {
            if (owner != null)
                owner._UnapplyAbility(this);
            level = lvl;
            localBehaviours = GetLocalBehaviours();
            globalBehaviours = GetGlobalBehaviours();
            modifier = GetModifier();
            if (owner != null)
                owner._ApplyAbility(this);
        }



        public NAbility(bool unique)
        {
            this.unique = unique;
            SetLevel(0);
        }
        internal void AttachAbility(NUnit owner)
        {
            this.owner = owner;
            OnAdded();
        }
        internal void DetachAbility()
        {
            OnRemoved();
            owner = null;
        }

        protected virtual void OnAdded()
        {
        }

        protected virtual void OnRemoved()
        {
        }

        public int CompareTo(NAbility other)
        {
            // descending order
            return other.level.CompareTo(level);
        }
    }
}