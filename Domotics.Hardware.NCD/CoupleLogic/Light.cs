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
            var ncdHardwareIdentifiers = ids as NCDHardwareIdentifier[] ?? ids.ToArray();
            yield return new NCDControlMessage(state == "On", ncdHardwareIdentifiers[0].Bank, ncdHardwareIdentifiers[0].Unit);
        }
    }
}