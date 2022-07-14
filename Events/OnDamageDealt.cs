using NoxRaven.Events;
using NoxRaven.UnitAgents;
using NoxRaven.Units;

namespace NoxRaven.Events
{
    public class OnDamageDealt : EventArgs
    {
        public NoxUnit target;
        public bool triggerOnHit;
        public bool triggerCrit;
        public bool noRecursion; // extra flag to prevent recursion calls
        public DamageType dmgtype;
        public DamageSource dmgsource;


        public float rawDamage;

        public float processedDamage;
        public float critChance;
        public float critDamage;
    }
}