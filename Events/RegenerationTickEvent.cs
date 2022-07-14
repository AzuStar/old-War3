using NoxRaven.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoxRaven.Events
{
    public class RegenerationTickEvent : EventArgs
    {
        public float HealthValue;
        public float ManaValue;
    }
}
