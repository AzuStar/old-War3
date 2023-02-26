using NoxRaven.Events;
using NoxRaven.UnitAgents;
using NoxRaven.Units;

namespace NoxRaven.Events
{
    public class OnDamageDealt : EventArgs
    {
        public NUnit target;
        public bool noRecursion; // extra flag to prevent recursion calls
        public DamageType dmgtype;
        public DamageSource dmgsource;
        public DamageOnHit dmgOnHit;
        public DamageCrit dmgCrit;


        public float rawDamage;

        public float processedDamage;
    }
}