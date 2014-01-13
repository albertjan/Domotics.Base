using System;
using System.Collections.Generic;
using System.Linq;
using Domotics.Base;

namespace Domotics.Hardware.NCD
{
    using Couple = Tuple<string, ICoupleLogic, IEnumerable<NCDHardwareIdentifier>>;

    public interface ICoupleLogic
    {
        IEnumerable<NCDControlMessage> GetMessage(State state, IEnumerable<NCDHardwareIdentifier> ids);
    }

    public class Light : ICoupleLogic
    {
        public IEnumerable<NCDControlMessage> GetMessage(State state, IEnumerable<NCDHardwareIdentifier> ids )
        {
            yield return new NCDControlMessage(state == "On", ids.First().Bank, ids.First().Unit);
        }
    }

    public class NCDEndPointCouplingInformation
    {
        public NCDEndPointCouplingInformation()
        {
            EndpointCouples = new List<Couple>();
        }

        public List<Couple> EndpointCouples { get; set; }
    }
}