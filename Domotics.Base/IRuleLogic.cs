using System.Collections.Generic;

namespace Domotics.Base
{
    /// <summary>
    /// the interface implemented by the dynamic code generated objects
    /// </summary>
    public interface IRuleLogic
    {
        /// <summary>
        /// Get the new state from this script
        /// </summary>
        /// <param name="input">new state of the "In" connection that caused this function to be called</param>
        /// <param name="connection">the connection that changed</param>
        /// <param name="connections">the connections that can be changed by this script</param>
        /// <param name="lastTriggered"> </param>
        /// <returns></returns>
        StateChangeDirective GetNewState(State input, Connection connection, List<Connection> connections, long lastTriggered);
    }
}