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

    public class Logic
    {
        private State Input { get; set; }
        private Connection Connection { get; set; }
        private List<Connection> Connections { get; set; }
        private long LastTriggered { get; set; }

        public Logic (State input, Connection connection, List<Connection> connections, long lastTriggered)
        {
            Input = input;
            Connection = connection;
            Connections = connections;
            LastTriggered = lastTriggered;
        }

        public ConnectionState When (string connectionName)
        {
            return new ConnectionState (Connections.First (c => c.Name == connectionName), Input, true, Connections, LastTriggered);
        }
    }

    public static class Extentions
    {
        public static ConnectionState IsPushed (this ConnectionState conState)
        {
            return conState.ChangesState ("in", "out");
        }

        public static ConnectionState IsHeld (this ConnectionState conState)
        {
            return conState.IsHeldFor(500);
        }

        public static ConnectionState IsHeldFor (this ConnectionState conState, int millisecs)
        {
            return conState.ChangesStateAfter ("in", "out", millisecs);
        }

        public static ConnectionState ChangesState (this ConnectionState conState, State from, State to)
        {
            if (conState.Item1.Type == ConnectionType.In || conState.Item1.Type == ConnectionType.Both)
            {
                return new ConnectionState(conState.Item1, conState.Item2,
                                           (conState.Item1.CurrentState == from && conState.Item2 == to), 
                                           conState.Item4,
                                           conState.Item5);
            }
            throw new LogicException ("Output Connections cant be \"Pushed\"");
        }
        
        public static ConnectionState ChangesStateAfter (this ConnectionState conState, State from, State to, int millisecs)
        {
            if (conState.Item1.Type == ConnectionType.In || conState.Item1.Type == ConnectionType.Both)
            {
                Debug.WriteLine ("LT: " + conState.Item5 + ", N: " + DateTime.Now.Ticks + ", D: " + (DateTime.Now.Ticks - conState.Item5) + ", " + (conState.Item5 + (millisecs * 10000) < DateTime.Now.Ticks));
                return new ConnectionState (conState.Item1, conState.Item2, (conState.Item1.CurrentState == from && conState.Item2 == to) && conState.Item5 + (millisecs * 10000) < DateTime.Now.Ticks, conState.Item4, conState.Item5);
            }
            throw new LogicException ("Output Connections cant be \"Pushed\"");
        }

        public static Func<string, IEnumerable<StateChangeDirective>> Turn (this ConnectionState connection, string which)
        {
            return s =>
            {
                if (!connection.Item3) return null;

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

    public class LogicException : Exception
    {
        public LogicException(string message) : base (message)
        {
            
        }
    }
}
