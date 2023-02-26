using NoxRaven.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoxRaven.Events
{
    public class OnRegeneration : EventArgs
    {
        public float healthRegen;
        public float manaRegen;
    }
}
