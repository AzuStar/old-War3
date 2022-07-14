using NoxRaven.Events;

namespace NoxRaven.Events
{
    public class OnLevelUp : EventArgs
    {
        public int previousLevel;
        public int newLevel;

    }
}