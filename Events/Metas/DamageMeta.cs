using NoxRaven.Units;

namespace NoxRaven.Events.Metas
{
    public class DamageMeta : EventMeta
    {
        public NoxUnit Source;
        public NoxUnit Target;
        public float Damage;
        public bool TriggerOnHit;
        public bool TriggerCrit;
        public bool IsSpell;
        public bool IsRanged;
        /// <summary>
        /// Exclusive flag for basic attack
        /// </summary>
        public bool IsBasicAttack; // reserved uncallable flag
        public bool StopRecursion; // extra flag to prevent recursion calls
        public EventLayer Layer = EventLayer.Simple;
 
    }
}