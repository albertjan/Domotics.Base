namespace Domotics.Hardware.NCD
{
    public class NCDHardwareIdentifier
    {
        public byte Bank { get; set; }

        public byte Unit { get; set; }

        public HardwareEndpointType Type { get; set; }
    }
}
