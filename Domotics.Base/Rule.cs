using System.Collections.Generic;
using System.Linq;

namespace Domotics.Base
{
    /// <summary>
    /// Checks wether or not to change state.
    /// </summary>
    public class Rule
    {

        /// <summary>
        /// TODO: fill connection by finding them by name from the external sources.
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="connections"></param>
        public Rule(string logic, IEnumerable<string> connections)
        {
            Logic = RuleLogicCompiler.Compile(logic);
            Connections = new List<Connection>();
            foreach (var connection in connections)
            {
                ((List<Connection>)Connections).Add(new Connection(connection));
            }
            //resolve connections and put them in the property.
        }

        /// <summary>
        /// The connections this rule does things with. 
        /// </summary>
        public IEnumerable<Connection> Connections { get; set; }

        private IRuleLogic Logic { get; set; }

        /// <summary>
        /// Checks wether the rules make sense.
        /// </summary>
        public bool Check ()
        {
            if (Logic == null) return false;
            //connections is not null
            if (Connections == null) return false;
            //must contain 2 elements
            if (Connections.Count () < 2) return false;
            //must contain altleast one in and one out 
            if (Connections.Any (c => c.Type == ConnectionType.In) && Connections.Any (c => c.Type == ConnectionType.Out))
                return true;
            //or one in and one both
            if (Connections.Any (c => c.Type == ConnectionType.In) && Connections.Any (c => c.Type == ConnectionType.Both))
                return true;
            //or one out and one both 
            if (Connections.Any (c => c.Type == ConnectionType.Out) && Connections.Any (c => c.Type == ConnectionType.Both))
                return true;
            //or two both
            return Connections.Count(c => c.Type == ConnectionType.Both) > 1;
        }

        /// <summary>
        /// Fire the rule to see if things will change.
        /// </summary>
        /// <param name="connection">the concerning connection</param>
        /// <param name="oldState">the previous state</param>
        public StateChangeDirective Fire(Connection connection, State oldState)
        {
            return Logic.GetNewState(oldState, connection, Connections.ToList());
        }
    }
}
