using System.Collections.Generic;
using Domotics.Base;

namespace Domotics.Hardware.NCD
{
    public interface ICoupleLogic
    {
        IEnumerable<NCDControlMessage> GetMessage(State state, IEnumerable<NCDHardwareIdentifier> ids);
    }
}