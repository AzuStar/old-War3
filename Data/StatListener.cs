using System;

namespace NoxRaven.Data
{
    public class StatListener : IDisposable
    {
        private NData _data;
        public int targetStatId;
        private NData.ListenerHandler callback;
        private bool isSuspended;

        internal StatListener(NData data, int targetStatId, NData.ListenerHandler callback)
        {
            _data = data;
            this.targetStatId = targetStatId;
            this.callback = callback;
            isSuspended = false;

            _data.RegisterListener(targetStatId, OnStatChanged);
        }

        public void Dispose()
        {
            _data.UnregisterListener(targetStatId, OnStatChanged);
        }

        public void Suspend()
        {
            isSuspended = true;
        }

        public void Resume()
        {
            isSuspended = false;
        }

        internal void OnStatChanged(NData.StatChange change)
        {
            if (!isSuspended)
            {
                callback(change);
            }
        }
    }

}