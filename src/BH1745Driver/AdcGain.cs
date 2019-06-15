namespace BH1745Driver
{
    /// <summary>
    /// Represents the available ADC gain options for the Bh1745.
    /// </summary>
    public enum AdcGain
    {
        /// <summary>
        /// Gain multiplier of 1.
        /// </summary>
        X1 = 0b00,
        /// <summary>
        /// Gain multiplier of 2.
        /// </summary>
        X2 = 0b01,
        /// <summary>
        /// Gain multiplier of 16.
        /// </summary>
        X16 = 0b10
    }
}
