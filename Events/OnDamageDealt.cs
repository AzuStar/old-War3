using NoxRaven.Events;
using NoxRaven.UnitAgents;

namespace NoxRaven.Events
{
    public class OnDamageDealt : EventArgs
    {
        public NAgent target;
        public bool noRecursion; // extra flag to prevent recursion calls
        public DamageType dmgtype;
        public DamageSource dmgsource;
        public DamageOnHit dmgOnHit;
        public DamageCrit dmgCrit;

        public float rawDamage;

        public float processedDamage;
    }
}
