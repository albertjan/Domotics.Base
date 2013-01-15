using System;
using System.Collections.Generic;
using System.Linq;
using Domotics.Base;

namespace Domotics.Web
{
    public class WebExternalSource : IExternalSource
    {
        public Action<Distributor> DistributorInitializationDelegate
        {
            get
            {
                return distributor =>
                {
                    Connections =
                        distributor.ExternalSources.Where(e => e != this).SelectMany(es => es.Connections).Select(
                            s => new Connection(s.Name)
                                 {
                                     Copied = true
                                 }).ToList();
                };
            }
        }

        public event ConnectionStateChangedEventHandler Input;
        public IEnumerable<Connection> Connections { get; private set; }
        public void SetState(Connection connection, string statename)
        {
            throw new System.NotImplementedException();
        }
    }
}