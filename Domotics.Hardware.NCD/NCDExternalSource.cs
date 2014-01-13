using System;
using System.Collections.Generic;
using Domotics.Base;

namespace Domotics.Hardware.NCD
{
    public class NCDExternalSource : IExternalSource
    {
        public NCDExternalSource()
        {
            controller = new NCDController(this);
            controller.Initialize();
        }
        
        private NCDController controller { get; set; } 

        public Action<Distributor> DistributorInitializationDelegate { get; private set; }
        public event ConnectionStateChangedEventHandler Input;
        public IObservable<ConnectionStateChangedEventHandlerArgs> StateChanges { get; private set; }
        public IEnumerable<Connection> Connections { get; private set; }
        public void SetState(Connection connectionid, string statename)
        {
            
        }
    }
}
