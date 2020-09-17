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
        public EventLayer Layer = EventLayer.Simple;
 
    }
}