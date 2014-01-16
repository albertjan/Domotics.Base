using System;
using System.Collections.Generic;
using System.Linq;
using Domotics.Base;

namespace Domotics.Hardware.NCD
{
    public class NCDExternalSource : IExternalSource
    {
        private IEnumerable<Connection> connections;

        public NCDExternalSource()
        {
            controller = new NCDController(this);
            controller.Initialize();
        } 
        
        private NCDController controller { get; set; } 

        public Action<Distributor> DistributorInitializationDelegate { get; private set; }
        public event ConnectionStateChangedEventHandler Input;
        public IObservable<ConnectionStateChangedEventHandlerArgs> StateChanges { get; private set; }
        public IEnumerable<Connection> Connections
        {
            get { return connections; } 
        }

        public void SetState(Connection connectionid, string statename)
        {
            controller.SetState(connectionid, (State)statename`);
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

        private void OnInput(ConnectionStateChangedEventHandlerArgs args)
        {
            if (Input != null)
            {
                Input(this, args);
            }
        }
    }
}