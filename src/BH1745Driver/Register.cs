using System;
using System.Collections.Generic;
using System.Text;

namespace BH1745Driver
{
    internal enum Register : byte
    {
        // control registers
        SYSTEM_CONTROL = 0x40,
        MODE_CONTROL1 = 0x41,
        MODE_CONTROL2 = 0x42,
        MODE_CONTROL3 = 0x44,

        // data registers
        RED_DATA_LSBs = 0x50,
        RED_DATA_MSBs = 0x51,
        GREEN_DATA_LSBs = 0x52,
        GREEN_DATA_MSBs = 0x53,
        BLUE_DATA_LSBs = 0x54,
        BLUE_DATA_MSBs = 0x55,
        CLEAR_DATA_LSBs = 0x56,
        CLEAR_DATA_MSBs = 0x57,
        DINT_DATA_LSBs = 0x58, // not necessary
        DINT_DATA_MSBs = 0x59, // not necessary

        // setting registers
        INTERRUPT = 0x60,
        PERSISTENCE = 0x61,

        // threshold registers
        TH_LSBs = 0x62,
        TH_MSBs = 0x63,
        TL_LSBs = 0x64,
        TL_MSBs = 0x65,

        // id register
        MANUFACTURER_ID = 0x92
    }
}
