using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Domotics.Base.DSL
{
    //Item1 = The connection that caused the rule to fire
    //Item2 = The new state
    //Item3 = A bool that causes the rule to return null when set to false. i.e. no change.
    //Item4 = LastTriggered in ticks.
    using ConnectionState = Tuple<Connection, State, bool, IEnumerable<Connection>, long>;

    /// <summary>
    /// The base for the user rule stories
    /// </summary>
    public class Logic
    {
        private State Input { get; set; }
        private Connection Connection { get; set; }
        private List<Connection> Connections { get; set; }
        private long LastTriggered { get; set; }
        
        /// <summary>
        /// The start of the story collecting all the 
        /// </summary>
        /// <param name="input">The state to which the input is to change after this story.</param>
        /// <param name="connection">The connection that caused this rule to fire.</param>
        /// <param name="connections">The connection that this rule talks about.</param>
        /// <param name="lastTriggered">The last time the rule was triggered.</param>
        public Logic (State input, Connection connection, List<Connection> connections, long lastTriggered)
        {
            Input = input;
            Connection = connection;
            Connections = connections;
            LastTriggered = lastTriggered;
        }

        /// <summary>
        /// The start of a rule story.
        /// </summary>
        /// <param name="connectionName">the name of the input you want it to react on.</param>
        /// <returns>the ConnectionState</returns>
        public ConnectionState When (string connectionName)
        {
            return new ConnectionState (Connections.First (c => c.Name == connectionName), Input, true, Connections, LastTriggered);
        }
    }

    /// <summary>
    /// Extension methods that make up the dsl.
    /// </summary>
    public static class Extentions
    {
        /// <summary>
        /// Continues the story when the input connection selected with When is pushed for less then 500 ms.
        /// </summary>
        /// <param name="conState">The state being passed around between the helper functions</param>
        /// <returns>The ConnectionState</returns>
        public static ConnectionState IsPushed (this ConnectionState conState)
        {
            return conState.ChangesStateWithin ("in", "out", 500);
        }

        /// <summary>
        /// Continues the story when the input connection selected with When is held for atleast 500ms.
        /// </summary>
        /// <param name="conState">the ConnectionState</param>
        /// <returns>the ConnectionState</returns>
        public static ConnectionState IsHeld (this ConnectionState conState)
        {
            return conState.IsHeldFor(500);
        }

        /// <summary>
        /// Continues the story when the input connection selected with When is held for the specfied number of milliseconds.
        /// </summary>
        /// <param name="conState">the ConnectionState</param>
        /// <param name="millisecs">the number of milliseconds after which the story is continued.</param>
        /// <returns>the ConnectionState</returns>
        public static ConnectionState IsHeldFor (this ConnectionState conState, int millisecs)
        {
            return conState.ChangesStateAfter ("in", "out", millisecs);
        }

        /// <summary>
        /// Continues the story when the input connection selected with When is pushed for less than the specified number of milliseconds and when the state changes from From to To.
        /// </summary>
        /// <param name="conState">the ConnectionState</param>
        /// <param name="from">from State</param>
        /// <param name="to">to State</param>
        /// <param name="millisecs">the specified number of milliseconds before which the state has to change.</param>
        /// <returns>the ConnectionState</returns>
        public static ConnectionState ChangesStateWithin (this ConnectionState conState, State from, State to, int millisecs)
        {
            if (conState.Item1.Type == ConnectionType.In || conState.Item1.Type == ConnectionType.Both)
            {
                Debug.WriteLine ("LT: " + conState.Item5 + ", N: " + DateTime.Now.Ticks + ", D: " + (DateTime.Now.Ticks - conState.Item5) + ", " + (conState.Item5 + (millisecs * 10000) > DateTime.Now.Ticks));

                return new ConnectionState(conState.Item1, conState.Item2,
                                           (conState.Item1.CurrentState == from && conState.Item2 == to) &&
                                           conState.Item5 + (millisecs*10000) > DateTime.Now.Ticks,
                                           conState.Item4,
                                           conState.Item5);
            }
            throw new LogicException ("Output Connections cant be \"Pushed\"");
        }

        /// <summary>
        /// Continues the story when the input connection selected with When is pushed for atleast the specified number of milliseconds and when the state changes from From to To.
        /// </summary>
        /// <param name="conState">the ConnectionState</param>
        /// <param name="from">from State</param>
        /// <param name="to">to State</param>
        /// <param name="millisecs">the specified number of milliseconds before which the state has to change.</param>
        /// <returns>the ConnectionState</returns>
        public static ConnectionState ChangesStateAfter (this ConnectionState conState, State from, State to, int millisecs)
        {
            if (conState.Item1.Type == ConnectionType.In || conState.Item1.Type == ConnectionType.Both)
            {
                Debug.WriteLine ("LT: " + conState.Item5 + ", N: " + DateTime.Now.Ticks + ", D: " + (DateTime.Now.Ticks - conState.Item5) + ", " + (conState.Item5 + (millisecs * 10000) < DateTime.Now.Ticks));
                return new ConnectionState(conState.Item1, conState.Item2,
                                           (conState.Item1.CurrentState == from && conState.Item2 == to) &&
                                           conState.Item5 + (millisecs*10000) < DateTime.Now.Ticks, conState.Item4,
                                           conState.Item5);
            }
            throw new LogicException ("Output Connections cant be \"Pushed\"");
        }

        /// <summary>
        /// Returns a func that can be called to return the StateChangeDirectives that are a result from the story.
        /// </summary>
        /// <param name="connection">the ConnectionState</param>
        /// <param name="which">a selector to select </param>
        /// <returns>the Func that can be called.</returns>
        public static Func<string, IEnumerable<StateChangeDirective>> Turn (this ConnectionState connection, string which)
        {
            return s =>
            {
                if (!connection.Item3) return new StateChangeDirective[] { null } ;

                return new[]
                           {
                               new StateChangeDirective
                                   {
                                       Connection = connection.Item4.First(c => c.Name == which),
                                       NewState = s
                                   }
                           };
            };
        }
    }

    /// <summary>
    /// An exception that fires when there is a logical error. For example: when an output connection is pushed.
    /// </summary>
    [Serializable]
    public class LogicException : Exception
    {
        /// <inherit-doc/>
        public LogicException(string message) : base (message)
        {
            
        }
    }
}
