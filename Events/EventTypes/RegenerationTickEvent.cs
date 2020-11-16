using NoxRaven.Events.Metas;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoxRaven.Events.EventTypes
{
    public class RegenerationTickEvent : Event
    {
        public RegenerationMeta EventInfo;

        public float HealthValue;
        public float ManaValue;
    }
}
