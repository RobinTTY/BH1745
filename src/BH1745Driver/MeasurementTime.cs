using System;
using System.Collections.Generic;
using System.Text;

namespace BH1745Driver
{
    public enum MeasurementTime : byte
    {
        /// <summary>
        /// 160ms measurement time.
        /// </summary>
        Ms160 = 0b000,
        /// <summary>
        /// 320ms measurement time.
        /// </summary>
        Ms320 = 0b001,
        /// <summary>
        /// 640ms measurement time.
        /// </summary>
        Ms640 = 0b010,
        /// <summary>
        /// 1280ms measurement time.
        /// </summary>
        Ms1280 = 0b011,
        /// <summary>
        /// 2560ms measurement time.
        /// </summary>
        Ms2560 = 0b100,
        /// <summary>
        /// 5120ms measurement time.
        /// </summary>
        Ms5120 = 0b101
    }
}
