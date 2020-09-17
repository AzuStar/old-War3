using NoxRaven.Events.Metas;

namespace NoxRaven.Events.EventTypes
{
    public class DamageEvent : Event
    {
        public DamageMeta EventInfo;

        public float ProcessedDamage;
        public float CritChance;
        public float CritDamage;
    }
}