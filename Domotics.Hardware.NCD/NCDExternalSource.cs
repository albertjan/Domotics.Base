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
        public event ConnectionStateChangedEventHandler Input;
        public IEnumerable<Connection> Connections { get; private set; }
        public void SetState(Guid connectionid, string statename)
        {
            throw new NotImplementedException();
        }
    }
}
