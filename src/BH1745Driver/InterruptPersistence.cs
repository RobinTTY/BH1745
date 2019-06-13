namespace BH1745Driver
{
    public enum InterruptPersistence : byte
    {
        /// <summary>
        /// Interrupt status is toggled at each measurement end.
        /// </summary>
        ToggleMeasurementEnd = 0b00,
        /// <summary>
        /// Interrupt status is updated at each measurement end.
        /// </summary>
        UpdateMeasurementEnd = 0b01,
        /// <summary>
        /// Interrupt status is updated if 4 consecutive threshold judgments are the same.
        /// </summary>
        UpdateConsecutiveX4 = 0b10,
        /// <summary>
        /// Interrupt status is updated if 8 consecutive threshold judgments are the same.
        /// </summary>
        UpdateConsecutiveX8 = 0b11
    }
}
