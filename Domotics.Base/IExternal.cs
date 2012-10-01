using System;
using System.Collections.Generic;

namespace Domotics.Base
{
    /// <summary>
    /// An external source something that handles changes and sends events when a change happens.
    /// </summary>
    public interface IExternal
    {

        /// <summary>
        /// event that fires when something has changed
        /// </summary>
        event ConnectionStateChangedEventHandler Input;

        /// <summary>
        /// list of connections for this external source
        /// </summary>
        IEnumerable<Connection> Connections { get; set; }

        void SetState(Guid connectionid, string statename);
    }

    public delegate void ConnectionStateChangedEventHandler(object sender, ConnectionStateChangedEventHandlerArgs args);

    public class ConnectionStateChangedEventHandlerArgs
    {
        /// <summary>
        /// the connection this event belongs to
        /// </summary>
        public Guid ConnectionID { get; set; }
        /// <summary>
        /// the current/new state
        /// </summary>
        public IState NewState { get; set; }
        /// <summary>
        /// old state
        /// </summary>
        public IState OldState { get; set; }
    }
}
