using Domotics.Base;

namespace Domotics.Hardware.NCD
{
    public class NCDHardwareIdentifier
    {
        public byte Bank { get; set; }

        public byte Unit { get; set; }

        public ConnectionType Type { get; set; }
    }
}  
