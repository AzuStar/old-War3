using System;
using System.Collections.Generic;
using NoxRaven.Data;
using NoxRaven.Events;

namespace NoxRaven.UnitAgents
{
    /// <summary>
    /// Base class for all abilities. <br />
    /// Constructor parsed with <see cref="NAgent"/>owner. <br />
    /// </summary>
    public abstract class NAbility : IComparable<NAbility>
    {
        // [LabelText("Ability"), ReadOnly, ShowInInspector]
        // public string name => GetType().Name;
        // Some data will be added by inheriting class
        public NAgent owner;
        public int level { get; private set; }
        public bool unique { get; private set; } = true;

        protected virtual NDataModifier GetModifier() => null;

        protected virtual List<IBehaviour> GetLocalBehaviours() => null;

        protected virtual List<IBehaviour> GetGlobalBehaviours() => null;

        protected virtual List<NDataDependency> GetDataDependencies() => null;

        #region Current Data
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

        ///<summary>
        /// Dependencies that will be attached with this ability
        ///</summary>
        internal List<NDataDependency> dataDependencies;

        ///<summary>
        /// Use this to add listeners to the owner's stats
        ///</summary>
        internal List<StatListener> statListeners;
        #endregion

        public void SetLevel(int lvl)
        {
            if (owner != null)
                owner._UnapplyAbility(this);
            level = lvl;
            CollectCurrentData();
            if (owner != null)
                owner._ApplyAbility(this);
        }

        public void CollectCurrentData()
        {
            localBehaviours = GetLocalBehaviours();
            globalBehaviours = GetGlobalBehaviours();
            modifier = GetModifier();
            dataDependencies = GetDataDependencies();
            statListeners = new List<StatListener>();
        }

        protected internal virtual void OnAdded() { }

        protected internal virtual void OnRemoved() { }

        protected internal virtual void OnApplied() { }

        protected internal virtual void OnUnapplied() { }

        public void ListenToStat(Enum id, NData.ListenerHandler listener)
        {
            ListenToStat(Convert.ToInt32(id), listener);
        }

        /// <summary>
        /// Use this to add listeners to the owner's stats, this will dispose them when the ability is removed
        /// </summary>
        public void ListenToStat(int id, NData.ListenerHandler listener)
        {
            statListeners.Add(owner.state.AddListener(id, listener));
        }

        public virtual int CompareTo(NAbility other)
        {
            // descending order
            return other.level.CompareTo(level);
        }
    }
}
