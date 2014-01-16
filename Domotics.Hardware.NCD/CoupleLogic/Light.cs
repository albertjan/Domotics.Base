using System.Collections.Generic;
using System.Linq;
using Domotics.Base;

namespace Domotics.Hardware.NCD.CoupleLogic
{
    public class Light : ICoupleLogic
    {
        public IEnumerable<NCDControlMessage> GetMessage(State state, IEnumerable<NCDHardwareIdentifier> ids )
        {
            yield return new NCDControlMessage(state == "On", ids.First().Bank, ids.First().Unit);
        }  
    }
}