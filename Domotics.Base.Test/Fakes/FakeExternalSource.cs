using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.PlatformServices;
using System.Threading;

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
                                          },
                                      new Connection("knopje3", ConnectionType.In)
                                          {
                                              CurrentState = "out",
                                              Live = true,
                                              AvailableStates = new List<State> { "in", "out" }
                                          },
                                      new Connection("lampje3", ConnectionType.Out)
                                          {
                                              CurrentState = "0",
                                              AvailableStates = new List<State> { "0", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" }
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
            var con = Connections.First(c => c.Name == connection.Name);
            con.CurrentState = statename;
        }

        public void FireInputEvent(string connectionName, string newstate)
        {
            var con = Connections.First(c => c.Name == connectionName);
            //con.CurrentState = newstate;

            OnInput(new ConnectionStateChangedEventHandlerArgs
                    {
                        Connection = con,
                        NewState = newstate
                    });
            
        }

        //private static Connection EventPumpConnection { get; set; }
        //private static IObservable<State> EventPump { get; set; }

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
