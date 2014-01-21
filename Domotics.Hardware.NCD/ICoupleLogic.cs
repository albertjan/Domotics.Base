using System.Collections.Generic;
using Domotics.Base;

namespace Domotics.Hardware.NCD
{
    public interface ICoupleLogic
    {
        IEnumerable<NCDControlMessage> GetMessages(State state, IEnumerable<NCDHardwareIdentifier> ids);
    }
}