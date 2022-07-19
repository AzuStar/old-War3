using System;

namespace NoxRaven.UnitAgents
{
    public interface IModifier
    {
        Stats ApplyModifier(Stats stats);
        Stats UnapplyModifier(Stats stats);
    }
    public sealed class Modifier<T> : IModifier where T : Stats
    {
        private T _stats;
        public Modifier(T stats)
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