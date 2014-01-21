namespace Domotics.Hardware.NCD.CoupleLogic
{
    using System.Collections.Generic;
    using Base;

    public class SixteenStateDimmer : ICoupleLogic
    {
        public IEnumerable<NCDControlMessage> GetMessages(State state, IEnumerable<NCDHardwareIdentifier> ids)
        {
            //0000 0%
            //0001  6 2/3
            //0010 13 1/3
            //0011 20
            //0100 26 2/3
            //0101 33 1/3
            //0110 40
            //0111 46 2/3
            //1000 53 1/3
            //1001 60
            //1010 66 2/3
            //1011 73 1/2
            //1100 80
            //1101 86 2/3
            //1110 93 1/3
            //1111 100%

            if (state == "0%")
            {
                
            } else if (state == "6.25%")
            {
                
            } else if (state == "12.5%")
            {
                
            } else if (state == "18.75%")
            {
                
            } else if (state == "25%")
            {
                
            } else if (state == "31.25%")
            {
                
            } else if (state == "37.5%")
            {
                
            } else if (state == "43.75%")
            {
                
            } else if (state == "50%")
            {
                
            } else if (state == "56.25%")
            {
                
            } else if (state == "62.5%")
            {
                
            } else if (state == "68.75%")
            {
            
            } else if (state == "75%") 
            {

            } else if (state == "81.25%")
            {
                
            } else if (state == "87.5%")
            {

            } else if (state == "93.75%")
            {
                
            } else if (state == "100%")
            {

            }
        }
    }
}