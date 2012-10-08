using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domotics.Base.Test.Fakes
{
    class FakeExternalSource : IExternalSource
    {
        public List<Connection> TestConnections { get; set; } 

        public event ConnectionStateChangedEventHandler Input;
        public IEnumerable<Connection> Connections { get; set; }
        public void SetState(Guid connectionid, string statename)
        {
            Connections.First(c => c.ID == connectionid).CurrentState = new State {Name = statename};
        }
    }
}
