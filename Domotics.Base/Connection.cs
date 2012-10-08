using System;
using System.Collections.Generic;

namespace Domotics.Base
{
    /// <summary>
    /// The connection to a piece of hardware. 
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// Name of the connection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// unique identifier
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Type of connections specifies capabilities.
        /// </summary>
        public ConnectionType Type { get; set; }

        /// <summary>
        /// List of available states for the connection
        /// </summary>
        public List<State> AvailableStates { get; set; }

        /// <summary>
        /// the current state
        /// </summary>
        public State CurrentState { get; set; }
    }
}
