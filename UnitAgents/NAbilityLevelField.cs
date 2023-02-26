using System.Collections.Generic;
using NoxRaven.Events;

namespace NoxRaven.UnitAgents
{
    public struct NAbilityLevelField
    {
        public readonly int level;

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

        public NAbilityLevelField(List<IBehaviour> localBehaviours, List<IBehaviour> globalBehaviours, IModifier mod, int level)
        {
            this.localBehaviours = localBehaviours;
            this.globalBehaviours = globalBehaviours;
            this.mod = mod;
            this.level = level;
        }

    }
}