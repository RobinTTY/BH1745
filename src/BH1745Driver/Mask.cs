using System;
using System.Collections.Generic;
using System.Text;

namespace BH1745Driver
{
    internal enum Mask : byte
    {
        PART_ID = 0x3F,
        SW_RESET = 0x80,
        INT_RESET = 0x40,

        MEASUREMENT_TIME = 0x07,
        VALID = 0x80,
        RGBC_EN = 0x10,
        ADC_GAIN = 0x03,

        INT_STATUS = 0x80,
        INT_LATCH = 0x10,
        INT_SOURCE = 0x0C,
        INT_ENABLE = 0x01,

        PERSISTENCE = 0x03,

        CLR = 0xFF
    }
}
