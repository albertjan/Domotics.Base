using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Domotics.Base
{
    /// <summary>
    /// Checks wether or not to change state.
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// The basic rule.
        /// </summary>
        /// <param name="logic">the story.</param>
        /// <param name="connections">and the connections it wan't to say something about.</param>
        public Rule(string logic, params string[] connections)
        {
            LogicText = logic;
            Logic = RuleLogicCompiler.Compile(logic);
            Connections = new List<Connection>();
            foreach (var connection in connections)
            {
                ((List<Connection>)Connections).Add(new Connection(connection));
            }
        }

        private string LogicText { get; set; }

        /// <summary>
        /// The connections this rule does things with. 
        /// </summary>
        public IEnumerable<Connection> Connections { get; set; }

        /// <summary>
        /// Compiled Logic.
        /// </summary>
        private IRuleLogic Logic { get; set; }

        /// <summary>
        /// ticks since 1-1-1970 
        /// </summary>
        public long LastTriggered { get; set; }

        /// <summary>
        /// ticks since 1-1-1970 to the moment the rule resulted in a statechange directive.
        /// </summary>
        public long TimeOfLastChange { get; set; }

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
        /// <param name="newState">the new state</param>
        public IEnumerable<StateChangeDirective> Fire(Connection connection, State newState)
        {
            Debug.WriteLine("Rule with logic: " + LogicText + " fireing!");
            var scd = Logic.GetNewState(newState, connection, Connections.ToList(), LastTriggered, TimeOfLastChange);
            LastTriggered = DateTime.Now.Ticks;
            if (scd.Any()) TimeOfLastChange = DateTime.Now.Ticks;
            return scd;
        }
    }
}
