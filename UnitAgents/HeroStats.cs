using System.Collections.Generic;

namespace NoxRaven.UnitAgents
{
    public class HeroStats : Stats
    {
        public float experienceGain { get => GetStat(47); set => SetStat(47, value); }
    }
}