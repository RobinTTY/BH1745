﻿using System;

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
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a not supported MeasurementTime is used.</exception>
        /// <returns></returns>
        public static int ToMilliseconds(this MeasurementTime time) =>
            time switch
            {
                MeasurementTime.Ms160 => 160,
                MeasurementTime.Ms320 => 320,
                MeasurementTime.Ms640 => 640,
                MeasurementTime.Ms1280 => 1280,
                MeasurementTime.Ms2560 => 2560,
                MeasurementTime.Ms5120 => 5120,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
