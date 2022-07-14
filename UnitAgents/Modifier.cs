using System;

namespace NoxRaven.UnitAgents
{
    public class Modifier
    {
        private Stats _stats;
        public Modifier(Stats stats)
        {
            _stats = stats;
        }

        public Stats ApplyModifier(Stats stats)
        {
            return (stats + _stats);
        }
        public Stats UnapplyModifier(Stats stats)
        {
            return (stats - _stats);
        }
    }
}