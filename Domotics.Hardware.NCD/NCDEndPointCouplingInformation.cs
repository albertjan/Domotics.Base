using System;
using System.Collections.Generic;
using Domotics.Base;
using Domotics.Hardware.NCD;


namespace Domotics.Hardware.NCD
{
    using Couple = Tuple<string, ICoupleLogic, IEnumerable<string>>;

    public interface ICoupleLogic
    {
        IEnumerable<NCDControlMessage> GetMessage(State state);
        IEnumerable<State> AvailableStates { get; }
    }

    public class Light : ICoupleLogic
    {
        public IEnumerable<NCDControlMessage> GetMessage(State state)
        {
            yield return new NCDControlMessage()
        }

        public IEnumerable<State> AvailableStates { get; set; }
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