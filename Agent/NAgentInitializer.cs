using System;
using System.Collections.Generic;
using NoxRaven.Data;
using NoxRaven.UnitAgents;

namespace NoxRaven
{
    public class NUnitInitializer
    {
        public NDataModifier starterStats;
        public List<NAbility> starterAbilities;
        public Action<NAgent> constructor;
    }
}
