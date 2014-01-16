using System;

namespace Domotics.Hardware.NCD
{
    [Serializable]
    public class RelayBank
    {
        public byte Number { get; set; }
        public byte AvailableRelays { get; set; }
    }
}
