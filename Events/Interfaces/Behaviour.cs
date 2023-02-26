using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NoxRaven.Events
{
    ///<summary>
    /// Behaves like a delegate Action<EventArguments>, except behaviours can be stacked according to their priority.
    ///</summary>
    public sealed class Behaviour<EvtType> : IBehaviour where EvtType : EventArgs
    {
        public Behaviour(Action<EvtType> action, PriorityType priority){
            _action = action;
            _priority = priority;
        }
        
        private PriorityType _priority = PriorityType.RESERVED_BEHAVIOUR;
        private Action<EvtType> _action = null;

        PriorityType IBehaviour.priority => _priority;

        public void GenericsInvoke(EventArgs eventArgs)
        {
            _action?.Invoke((EvtType)eventArgs);
        }

    }

}