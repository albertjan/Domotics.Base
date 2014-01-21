using System.Collections.Generic;
using System.Linq;
using Domotics.Base;

namespace Domotics.Hardware.NCD.CoupleLogic
{
    public class Light : ICoupleLogic
    {
        public ushort NumberOfRelaysNeeded { get { return 1; } }

        public IEnumerable<NCDControlMessage> GetMessages(State state, IEnumerable<NCDHardwareIdentifier> ids )
        {
            yield return new NCDControlMessage(state == "On", ids.First().Bank, ids.First().Unit);
        }
    }
}