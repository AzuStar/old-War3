using NoxRaven.Units;

namespace NoxRaven.Events
{
    public class OnDeath : EventArgs
    {
        public NUnit killer;
        public float keepCorpseFor;
        public bool keepCorpse;
    }
}