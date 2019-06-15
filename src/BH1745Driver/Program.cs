using System;
using System.Device.I2c;
using System.Device.I2c.Drivers;
using System.Linq;
using System.Threading.Tasks;

namespace BH1745Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            //var debug = true;
            //while (debug)
            //    Task.Delay(1000).Wait();

            Console.WriteLine("Hello BH1745!");

            //bus id on the raspberry pi 3
            const int busId = 1;

            var i2cSettings = new I2cConnectionSettings(busId, Bh1745.DefaultI2cAddress);
            var i2cDevice = new UnixI2cDevice(i2cSettings);
            var i2CBh1745 = new Bh1745(i2cDevice);

            i2CBh1745.Init();
            i2CBh1745.InterruptLatch = true;
            i2CBh1745.InterruptIsEnabled = true;
            i2CBh1745.MeasurementTime = MeasurementTime.Ms320;

            Console.WriteLine("All Bh1745 properties:");
            typeof(Bh1745).GetProperties().ToList().ForEach(property =>
            {
                Console.WriteLine(property.Name + ": " + property.GetValue(i2CBh1745));
            });

            for (var i = 0; i < 5; i++)
            {
                TakeMeasurement(i2CBh1745);
            }

            while (true)
            {
                TakeMeasurement(i2CBh1745);
            }
        }

        private static void TakeMeasurement(Bh1745 sensor)
        {
            var uncompensatedColor = sensor.GetUncompensatedColor();
            var bh1745Color = sensor.GetCompensatedColor();
            var color = sensor.GetCompensatedColor().ToColor();

            Console.WriteLine("red:{0} green:{1} blue:{2} clear:{3}",
                uncompensatedColor.Red, uncompensatedColor.Green, uncompensatedColor.Blue, uncompensatedColor.Clear);

            Console.WriteLine("red:{0} green:{1} blue:{2} clear:{3}",
                bh1745Color.Red, bh1745Color.Green, bh1745Color.Blue, bh1745Color.Clear);

            Console.WriteLine("RGB color read: #{0:X}{1:X}{2:X}", color.R, color.G, color.B);
            Console.WriteLine();
            Task.Delay(sensor.MeasurementTime.ToMilliseconds()).Wait();
        }
    }
}
