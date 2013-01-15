using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
                                              CurrentState = "off",
                                              AvailableStates = new List<State> { "on", "off" }
                                          },
                                      new Connection("lampje2", ConnectionType.Out)
                                          {
                                              CurrentState = "off",
                                              AvailableStates = new List<State> { "on", "off" }
                                          },
                                      new Connection("knopje", ConnectionType.In)
                                          {
                                              CurrentState = "out",
                                              AvailableStates = new List<State> { "in", "out" }
                                          },
                                      new Connection("knopje2", ConnectionType.In)
                                          {
                                              CurrentState = "out",
                                              AvailableStates = new List<State> { "in", "out" }
                                          }
                                  };
        }

        private List<Connection> TestConnections { get; set; }

        public Action<Distributor> DistributorInitializationDelegate { get; private set; }

        public event ConnectionStateChangedEventHandler Input;
        public IEnumerable<Connection> Connections { get { return TestConnections; } }
        public void SetState(Connection connection, string statename)
        {
            Debug.WriteLine("Setting: " + connection.Name + " to: " + statename);
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
