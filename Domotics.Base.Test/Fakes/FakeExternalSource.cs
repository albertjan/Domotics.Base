using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domotics.Base.Test.Fakes
{
    class FakeExternalSource : IExternalSource
    {
        public FakeExternalSource()
        {
            TestConnections = new List<Connection>
                                  {
                                      new Connection("lampje", ConnectionType.Out)
                                          {
                                              CurrentState = "off"
                                          },
                                      new Connection("knopje", ConnectionType.In)
                                          {
                                              CurrentState = "out"
                                          }
                                  };
        }

        public List<Connection> TestConnections { get; set; } 

        public event ConnectionStateChangedEventHandler Input;
        public IEnumerable<Connection> Connections { get { return TestConnections; } }
        public void SetState(Connection connection, string statename)
        {
            Connections.First(c => c.Name == connection.Name).CurrentState = statename;
        }

        public void FireInputEvent(string connectionName, string newstate)
        {
            OnInput(new ConnectionStateChangedEventHandlerArgs
                        {
                            Connection = Connections.First(c => c.Name == connectionName),
                            NewState = newstate
                        });
        }
       
        private void OnInput(ConnectionStateChangedEventHandlerArgs args)
        {
            if (Input != null)
            {
                Input(this, args);
            }
        }
    }
}
