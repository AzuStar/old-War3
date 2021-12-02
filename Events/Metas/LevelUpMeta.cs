using NoxRaven.Units;

namespace NoxRaven.Events.Metas
{
    public class LevelUpMeta : EventMeta
    {
        public NoxHero Hero;

        public int PreviousLevel;
        public int NewLevel;
    }
}