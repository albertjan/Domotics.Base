namespace Domotics.Hardware.NCD
{
    public class NCDControlMessage
    {
        /// <summary>
        ///  on/off banknumber    relay
        ///  0-1    0-32          0-7
        /// | 0000 | 0000 | 0000 | 0000 |
        /// 
        /// turn relay 4 on bank 3 on
        /// | 0001 | 0000 | 0011 | 0100 |
        /// 
        /// 16 to 12         | 12 to 4      | 4 to 0
        /// Status 1 bit max | bank 255 max | relay 8 max
        /// </summary>
        /// <param name="status"></param>
        /// <param name="bank"></param>
        /// <param name="relay"></param>
        public NCDControlMessage(bool status, byte bank, byte relay)
        {
            Bank = bank;
            Status = (byte) (status ? 1 : 0);
            Relay = relay;
        }

        private byte Bank { get; set; }

        private byte Relay { get; set; }

        private byte Status { get; set; }

        public ushort GetMessage()
        {
            return (ushort) ((Status << 12) + (Bank << 4) + Relay);
        }

        public override string ToString ()
        {
            return "(Bank: " + Bank + " Relay: " + Relay + " Status: " + Status + ")";
        }
    }
}
