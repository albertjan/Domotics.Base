namespace Domotics.Hardware.NCD.CoupleLogic
{
    using System.Collections.Generic;
    using Base;

    public interface ICoupleLogic
    {
        ushort NumberOfRelaysNeeded { get; }
        IEnumerable<NCDControlMessage> GetMessages(State state, IEnumerable<NCDHardwareIdentifier> ids);
    }
}