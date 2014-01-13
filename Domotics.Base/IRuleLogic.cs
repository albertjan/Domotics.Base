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
        /// <param name="timetimeOfLastChange">Last time this rule caused a change</param>
        /// <returns>StateChanges</returns>
        IEnumerable<StateChangeDirective> GetNewState(State input, 
                                                      Connection connection, 
                                                      List<Connection> connections,
                                                      long lastTriggered, 
                                                      long timetimeOfLastChange);

        /// <summary>
        /// Contains the path the the compiled assembly
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Contains the string that is compiled.
        /// </summary>
        string Logic { get; }
    }
}