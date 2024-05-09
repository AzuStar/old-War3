namespace NoxRaven.Events
{
    public class OnDeath : EventArgs
    {
        public NAgent killer;
        public float keepCorpseFor;
        public bool keepCorpse;
    }
}
