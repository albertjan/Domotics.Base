using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domotics.Base.DSL
{
    public class Logic
    {
        private State Input { get; set; }
        private Connection Connection { get; set; }
        private List<Connection> Connections { get; set; }

        public Logic (State input, Connection connection, List<Connection> connections)
        {
            Input = input;
            Connection = connection;
            Connections = connections;
        }

        public Tuple<Connection, State, IEnumerable<Connection>> When (string connectionName)
        {
            return new Tuple<Connection, State, IEnumerable<Connection>>(Connections.First(c => c.Name == connectionName), Input, Connections);
        }
    }

    public static class Extentions
    {
        public static Tuple<Connection, bool, IEnumerable<Connection>> IsPushed (this Tuple<Connection, State, IEnumerable<Connection>> conState)
        {
            if (conState.Item1.Type == ConnectionType.In || conState.Item1.Type == ConnectionType.Both)
            {
                return new Tuple<Connection, bool, IEnumerable<Connection>> (conState.Item1, (conState.Item1.CurrentState == "in" && conState.Item2 == "out"), conState.Item3);
            }        
            throw new LogicException("Output Connections cant be \"Pushed\"");
        }

        public static Func<string, StateChangeDirective> Turn (this Tuple<Connection, bool, IEnumerable<Connection>> connection, string which)
        {
            return s => {
                if (connection.Item2) return null;
                
                return new StateChangeDirective
                    {
                        Connection = connection.Item3.First(c => c.Name == which),
                        NewState = s
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
