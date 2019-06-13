using System;
using System.Collections.Generic;
using System.Text;

namespace BH1745Driver
{
    public static class Bh1745Extensions
    {
        public static int ToMilliseconds(this MeasurementTime time)
        {
            switch (time)
            {
                case MeasurementTime.Ms160:
                    return 160;
                case MeasurementTime.Ms320:
                    return 320;
                case MeasurementTime.Ms640:
                    return 640;
                case MeasurementTime.Ms1280:
                    return 1280;
                case MeasurementTime.Ms2560:
                    return 2560;
                case MeasurementTime.Ms5120:
                    return 5120;
                default:
                    throw new ArgumentOutOfRangeException(nameof(time), time, null);
            }
        }
    }
}
