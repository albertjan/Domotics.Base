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

        public Connection When(string connectionName)
        {
            return Connections.First(c => c.Name == connectionName);
        }
    }

    public static class Extentions
    {
        public static Connection IsPushed(this Connection con)
        {
            if (con.Type == ConnectionType.In || con.Type == ConnectionType.Both)
            {
                return con;
            }        
            throw new LogicException("Output Connections cant be \"Pushed\"");
        }
    }

    public class LogicException : Exception
    {
        public LogicException(string message) : base (message)
        {
            
        }
    }
}
