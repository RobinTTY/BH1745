using System;

namespace BH1745Driver
{
    /// <summary>
    /// Extension methods for the Bh1745 sensor.
    /// </summary>
    public static class Bh1745Extensions
    {
        /// <summary>
        /// Converts the enum Measurement time to an integer representing the measurement time in ms.
        /// </summary>
        /// <param name="time">The MeasurementTime.</param>
        /// <returns></returns>
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
