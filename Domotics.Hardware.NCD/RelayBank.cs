using System;

namespace Domotics.Hardware.NCD
{
    [Serializable]
    public class RelayBank
    {
        public int Number { get; set; }
        public int AvailableRelays { get; set; }
    }
}
