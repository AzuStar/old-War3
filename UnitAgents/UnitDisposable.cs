using System;

namespace NoxRaven.UnitAgents
{
    public abstract class UnitDisposable : IDisposable
    {
        public abstract void Update();
        public abstract void Dispose();
    }
}