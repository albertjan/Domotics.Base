using System;
using System.Collections.Generic;

namespace Domotics.Base
{
    /// <summary>
    /// An external source something that handles changes and sends events when a change happens.
    /// </summary>
    public interface IExternalSource
    {
        /// <summary>
        /// Is called after the distributor is initialized. 
        /// </summary>
        Action<Distributor> DistributorInitializationDelegate { get; }

        /// <summary>
        /// event that fires when something has changed
        /// </summary>
        event ConnectionStateChangedEventHandler Input;

        /// <summary>
        /// Obserable of all state change events
        /// </summary>
        IObservable<ConnectionStateChangedEventHandlerArgs> StateChanges { get; }

        /// <summary>
        /// list of connections for this external source
        /// </summary>
        IEnumerable<Connection> Connections { get; }

        /// <summary>
        /// Set the state for a connection
        /// </summary>
        /// <param name="connection">the connection</param>
        /// <param name="statename">the state</param>
        void SetState(Connection connection, string statename);
    }

    /// <summary>
    /// The event that fires when a state changes in this source
    /// </summary>
    /// <param name="sender">this</param>
    /// <param name="e">the <see cref="ConnectionStateChangedEventHandlerArgs"/>Arguments</param>
    public delegate void ConnectionStateChangedEventHandler(object sender, ConnectionStateChangedEventHandlerArgs e);

    /// <summary>
    /// The arguments for the connectionstatechanged events.
    /// </summary>
    public class ConnectionStateChangedEventHandlerArgs : EventArgs
    {
        /// <summary>
        /// the connection this event belongs to
        /// </summary>
        public Connection Connection { get; set; }
        /// <summary>
        /// the current/new state
        /// </summary>
        public State NewState { get; set; }
    }
}
