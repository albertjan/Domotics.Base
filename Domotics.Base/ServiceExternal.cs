using System;
using System.Collections.Generic;

namespace Domotics.Base
{
    public class ServiceExternal : IExternalSource
    {
        private readonly List<Connection> _connections;

        public ServiceExternal ()
        {
            _connections = new List<Connection>();
        }

        public event ConnectionStateChangedEventHandler Input;

        public IEnumerable<Connection> Connections
        {
            get { return _connections; }
            set { _connections.AddRange(value); }
        }

        public void SetState(Guid connectionid, string statename)
        {
            throw new NotImplementedException();
        }
    }
}