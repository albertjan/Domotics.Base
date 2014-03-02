namespace Domotics.Hardware.NCD.CoupleLogic
{
    using System.Collections.Generic;
    using System.Linq;
    using Base;

    public class SixteenStateDimmer : ICoupleLogic
    {
        public ushort NumberOfRelaysNeeded { get { return 4; } }

        public IEnumerable<NCDControlMessage> GetMessages(State state, IEnumerable<NCDHardwareIdentifier> ids)
        {
            var id = ids.ToArray();
            if (state == "0%")
            {
                //0000 0%
                yield return new NCDControlMessage(false, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(false, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(false, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(false, id[3].Bank, id[3].Unit);
            }
            else if (state == "6.66%")
            {
                //0001  6 2/3
                yield return new NCDControlMessage(false, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(false, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(false, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(true, id[3].Bank, id[3].Unit);
            }
            else if (state == "13.33%")
            {
                //0010 13 1/3
                yield return new NCDControlMessage(false, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(false, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(true, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(false, id[3].Bank, id[3].Unit);
            }
            else if (state == "20%")
            {
                //0011 20
                yield return new NCDControlMessage(false, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(false, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(true, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(true, id[3].Bank, id[3].Unit);
            }
            else if (state == "26.66%")
            {
                //0100 26 2/3
                yield return new NCDControlMessage(false, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(true, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(false, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(false, id[3].Bank, id[3].Unit);
            }
            else if (state == "33.33%")
            {
                //0101 33 1/3
                yield return new NCDControlMessage(false, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(true, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(false, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(true, id[3].Bank, id[3].Unit);
            }
            else if (state == "40%")
            {
                //0110 40
                yield return new NCDControlMessage(false, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(true, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(true, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(false, id[3].Bank, id[3].Unit);
            }
            else if (state == "46.66%")
            {
                //0111 46 2/3
                yield return new NCDControlMessage(false, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(true, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(true, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(true, id[3].Bank, id[3].Unit);
            }
            else if (state == "53.33%")
            {
                //1000 53 1/3
                yield return new NCDControlMessage(true, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(false, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(false, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(false, id[3].Bank, id[3].Unit);
            }
            else if (state == "60%")
            {
                //1001 60
                yield return new NCDControlMessage(true, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(false, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(false, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(true, id[3].Bank, id[3].Unit);
            }
            else if (state == "66.66%")
            {
                //1010 66 2/3
                yield return new NCDControlMessage(true, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(false, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(true, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(false, id[3].Bank, id[3].Unit);
            }
            else if (state == "73.33%")
            {
                //1011 73 1/3
                yield return new NCDControlMessage(true, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(false, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(true, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(true, id[3].Bank, id[3].Unit);
            }
            else if (state == "80%")
            {
                //1100 80
                yield return new NCDControlMessage(true, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(true, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(false, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(false, id[3].Bank, id[3].Unit);
            }
            else if (state == "86.66%")
            {
                //1101 86 2/3
                yield return new NCDControlMessage(true, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(true, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(false, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(true, id[3].Bank, id[3].Unit);
            }
            else if (state == "93.33%")
            {
                //1110 93 1/3
                yield return new NCDControlMessage(true, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(true, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(true, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(false, id[3].Bank, id[3].Unit);
            }
            else if (state == "100%")
            {
                //1111 100
                yield return new NCDControlMessage(true, id[0].Bank, id[0].Unit);
                yield return new NCDControlMessage(true, id[1].Bank, id[1].Unit);
                yield return new NCDControlMessage(true, id[2].Bank, id[2].Unit);
                yield return new NCDControlMessage(true, id[3].Bank, id[3].Unit);
            }
        }
    }
} 