using System.Collections.Generic;

namespace Domotics.Base
{
    /// <summary>
    /// The connection to a piece of hardware. 
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// Create a new connection
        /// </summary>
        /// <param name="connectionName">thed name of the connection</param>
        public Connection (string connectionName)
        {
            Name = connectionName;
        }

        /// <summary>
        /// Create a new connection
        /// </summary>
        /// <param name="connectionName">the name of the connection</param>
        /// <param name="type">the ConnectionType</param>
        public Connection(string connectionName, ConnectionType type)
        {
            Name = connectionName;
            Type = type;
        }

        /// <summary>
        /// Name of the connection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies wether or not this connection can be resolved. When it is set to true
        /// the distributer will asume there is another connection from a different external
        /// source.
        /// </summary>
        public bool Copied { get; set; }
/*
        Don't know if this is necessairy. It's not logical to have connections with conflicting names.
        /// <summary>
        /// unique identifier
        /// </summary>
        public Guid ID { get; set; }
*/

        /// <summary>
        /// Type of connections specifies capabilities.
        /// </summary>
        public ConnectionType Type { get; private set; }

        /// <summary>
        /// List of available states for the connection
        /// </summary>
        public List<State> AvailableStates { get; set; }

        /// <summary>
        /// the current state
        /// </summary>
        public State CurrentState { get; set; }

        /// <summary>
        /// Specifies whether this button should fire live events should only be used with buttons you want
        /// to use with dimmable lights. It's more resource intensive.
        /// </summary>
        public bool Live { get; set; }

        /// <inherit-doc/>
        public override string ToString ()
        {
            return Name;
        }
    }
}