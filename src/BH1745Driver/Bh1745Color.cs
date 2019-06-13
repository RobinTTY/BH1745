using System;
using System.Drawing;

namespace BH1745Driver
{
    public class Bh1745Color
    {
        /// <summary>
        /// Red color channel value.
        /// </summary>
        public double Red { get; }
        /// <summary>
        /// Green color channel value.
        /// </summary>
        public double Green { get; }
        /// <summary>
        /// Blue color channel value.
        /// </summary>
        public double Blue { get; }
        /// <summary>
        /// Clear color channel value.
        /// </summary>
        public double Clear { get; }

        /// <summary>
        /// RGBC value representing a color.
        /// </summary>
        /// <param name="red">The red color portion.</param>
        /// <param name="green">The green color portion.</param>
        /// <param name="blue">The blue color portion.</param>
        /// <param name="clear">The clear color portion.</param>
        public Bh1745Color(double red, double green, double blue, double clear)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Clear = clear;
        }

        /// <summary>
        /// Returns a BH1745Color scaled against the clear channel.
        /// </summary>
        /// <returns></returns>
        public Bh1745Color GetScaled()
        {
            if (!(Clear > 0))
                return new Bh1745Color(0, 0, 0, 0);

            var redScaled = Math.Min(255, Red / Clear * 255);
            var greenScaled = Math.Min(255, Green / Clear * 255);
            var blueScaled = Math.Min(255, Blue / Clear * 255);

            return new Bh1745Color(redScaled, greenScaled, blueScaled, Clear);
        }

        public Color ToColor()
        {
            var scaledColor = GetScaled();
            return Color.FromArgb((int)scaledColor.Red, (int)scaledColor.Green, (int)scaledColor.Blue);
        }
    }
}
