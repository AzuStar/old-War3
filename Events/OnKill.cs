using NoxRaven.Events;
using NoxRaven.Units;

namespace NoxRaven.Events
{
    public class OnKill : EventArgs
    {
        public NoxUnit target;
    }
}