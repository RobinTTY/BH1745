using System;
using System.Device.I2c;
using System.Device.I2c.Drivers;
using System.Threading.Tasks;

namespace BH1745Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello BH1745!");

            //bus id on the raspberry pi 3
            const int busId = 1;

            var i2cSettings = new I2cConnectionSettings(busId, Bh1745.DefaultI2cAddress);
            var i2cDevice = new UnixI2cDevice(i2cSettings);
            var i2CBh1745 = new Bh1745(i2cDevice);

            while (true)
            {
                var uncompensatedColor = i2CBh1745.GetUncompensatedColor();
                var bh1745Color = i2CBh1745.GetCompensatedColor();
                var color = i2CBh1745.GetCompensatedColor().ToColor();

                Console.WriteLine("Uncompensated Bh1745 color read: #{0:X}{1:X}{2:X} clear:{3:X}",
                    uncompensatedColor.Red, uncompensatedColor.Green, uncompensatedColor.Blue, uncompensatedColor.Clear);

                Console.WriteLine("Compensated Bh1745 color read: #{0:X}{1:X}{2:X} clear:{3:X}",
                    bh1745Color.Red, bh1745Color.Green, bh1745Color.Blue, bh1745Color.Clear);

                Console.WriteLine("RGB color read: #{0:X}{1:X}{2:X}", color.R, color.G, color.B);

                Task.Delay(i2CBh1745.MeasurementTime.ToMilliseconds());
            }
        }
    }
}
