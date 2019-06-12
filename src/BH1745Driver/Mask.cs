using System;
using System.Collections.Generic;
using System.Text;

namespace BH1745Driver
{
    internal enum Mask : byte
    {
        PART_ID = 0x3F,
        SW_RESET = 0x80,

        MEASUREMENT_TIME = 0x07,
        VALID = 0x80,
        RGBC_EN = 0x10,
        ADC_GAIN = 0x03,

        CLR = 0xFF
    }
}
