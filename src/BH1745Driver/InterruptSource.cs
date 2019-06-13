namespace BH1745Driver
{
    public enum InterruptSource : byte
    {
        /// <summary>
        /// The red color channel.
        /// </summary>
        RedChannel = 0b00,
        /// <summary>
        /// The green color channel.
        /// </summary>
        GreenChannel = 0b01,
        /// <summary>
        /// The blue color channel.
        /// </summary>
        BlueChannel = 0b10,
        /// <summary>
        /// The clear color channel. 
        /// </summary>
        ClearChannel = 0b11
    }
}
