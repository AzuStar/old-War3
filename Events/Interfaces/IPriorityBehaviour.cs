using System;
using System.Collections.Generic;
using System.Diagnostics;
using NoxRaven.Events;

namespace NoxRaven.Events
{
    /// <summary>
    /// Interface for storing genric types
    /// </summary>
    public interface IPriorityBehaviour
    {
        PriorityType priority { get; }
        void GenericsInvoke(EventArgs args); // generics bypass
    }

    // Pilot behaviour is lower than then ship behaviour.
    public enum PriorityType : int
    {
        ///<summary>
        /// Misc priority (system events)
        ///</summary>
        RESERVED_BEHAVIOUR = 0,
        ///<summary>
        /// Just default behaviour
        ///</summary>
        PLAIN_BEHAVIOUR,
        ///<summary>
        /// Replace/Destroy behaviours
        ///</summary>
        PLAIN_OVERRIDED_BEHAVIOUR,
        ///<summary>
        /// Extra
        ///</summary>
        EXTRA_BEHAVIOUR,
        ///<summary>
        /// Replace/Destroy behaviours
        ///</summary>
        EXTRA_OVERRIDED_BEHAVIOUR,
        ///<summary>
        /// The most distant priority, use this or next 1000 blocks
        ///</summary>
        TAIL_BEHAVIOUR = 1000,
        /// System 
        RESERVED_TAIL_BEHAVIOUR = 2000,
    }
}