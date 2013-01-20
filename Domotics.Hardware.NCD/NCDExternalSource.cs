using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domotics.Base;

namespace Domotics.Hardware.NCD
{
    public class NCDExternalSource : IExternalSource
    {
        public Action<Distributor> DistributorInitializationDelegate { get; private set; }
        public event ConnectionStateChangedEventHandler Input;
        public IEnumerable<Connection> Connections { get; private set; }
        public void SetState(Connection connectionid, string statename)
        {
            throw new NotImplementedException();
        }
    }
}
