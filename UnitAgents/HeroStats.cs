using System.Collections.Generic;

namespace NoxRaven.UnitAgents
{
    /// <summary>
    /// Use 50 (inclusive), for custom stats
    /// </summary>
    public class HeroStats : Stats
    {
        public float experienceGain { get => GetStat(49); set => SetStat(49, value); }
    }
}