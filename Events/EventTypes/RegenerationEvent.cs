using NoxRaven.Events.Metas;

namespace NoxRaven.Events.EventTypes
{
    public class RegenerationEvent : Event
    {
        public RegenerationMeta EventInfo;
        public float PredictedRegeneration;
    }
}