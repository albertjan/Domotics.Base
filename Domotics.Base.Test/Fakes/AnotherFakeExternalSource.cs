using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Domotics.Base.Test.Fakes
{
    class AnotherFakeExternalSource : IExternalSource
    {
        public AnotherFakeExternalSource()
        {
            TestConnections = new List<Connection>
                                  {
                                      new Connection("lampje2", ConnectionType.Out)
                                          {
                                              CurrentState = "off",
                                              AvailableStates = new List<State> { "on", "off" }
                                          },
                                      //new Connection("lampje3", ConnectionType.Out)
                                      //    {
                                      //        CurrentState = "off",
                                      //        AvailableStates = new List<State> { "on", "off" }
                                      //    },
                                      new Connection("knopje", ConnectionType.In)
                                          {
                                              CurrentState = "out",
                                              AvailableStates = new List<State> { "in", "out" },
                                              Copied = true
                                          },
                                      new Connection("knopje3", ConnectionType.In)
                                          {
                                              CurrentState = "out",
                                              AvailableStates = new List<State> { "in", "out" }
                                          }
                                  };
        }

        private List<Connection> TestConnections { get; set; }

        public Action<Distributor> DistributorInitializationDelegate { get; private set; }

        public event ConnectionStateChangedEventHandler Input;
        public IObservable<ConnectionStateChangedEventHandlerArgs> StateChanges { get; private set; }
        public IEnumerable<Connection> Connections { get { return TestConnections; } }
        public void SetState(Connection connection, string statename)
        {
            Debug.WriteLine(this + " Setting: " + connection.Name + " to: " + statename);
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

        public Action<Distributor> MyDistributorInitializationDelegate
        {
            set { DistributorInitializationDelegate = value; }
        }
    }
}
