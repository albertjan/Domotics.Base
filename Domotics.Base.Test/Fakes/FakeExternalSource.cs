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
                                      new Connection("lampje")
                                          {
                                              Type = ConnectionType.Out
                                          },
                                      new Connection("knopje")
                                          {
                                              Type = ConnectionType.In
                                          }
                                  };
        }

        public List<Connection> TestConnections { get; set; } 

        public event ConnectionStateChangedEventHandler Input;
        public IEnumerable<Connection> Connections { get { return TestConnections; } }
        public void SetState(Guid connectionid, string statename)
        {
            Connections.First(c => c.ID == connectionid).CurrentState = new State {Name = statename};
        }
    }
}
