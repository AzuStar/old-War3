using NoxRaven.Events;
using NoxRaven.Units;

namespace NoxRaven.Events
{
    public class KillEvent : EventArgs
    {
        public NoxUnit target;
    }
}