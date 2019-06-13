// TODO: clarify API surface
// TODO: headers, examples

using System;
using System.Buffers.Binary;
using System.Device.I2c;
using System.Diagnostics;
using System.Linq;

namespace BH1745Driver
{
    public class Bh1745 : IDisposable
    {
        /// <summary>
        /// The primary I2c address of the BH1745
        /// </summary>
        public static byte DefaultI2cAddress = 0x38;

        /// <summary>
        /// The secondary I2c address of the BH1745
        /// </summary>
        public static byte SecondaryI2cAddress = 0x39;

        /// <summary>
        /// Gets or sets the state of the software reset.
        /// True indicates that the initial reset is started, false that it is not started.
        /// On reset all registers are reset and the sensor goes to power down mode.
        /// </summary>
        public bool SoftwareReset
        {
            get
            {
                var status = Read8BitsFromRegister((byte)Register.SYSTEM_CONTROL);
                status = (byte)(status & (byte)Mask.SW_RESET);
                return Convert.ToBoolean(status);
            }
            set
            {
                var status = Read8BitsFromRegister((byte) Register.SYSTEM_CONTROL);
                status = (byte) (status & ((byte) Mask.SW_RESET ^ (byte) Mask.CLR));
                status = (byte) (status | 0x01 << 7);

                Write8BitsToRegister((byte) Register.SYSTEM_CONTROL, status);
            }
        }

        /// <summary>
        /// Gets or sets the state of the interrupt pin.
        /// False is the default state in which the interrupt pin is not initialized (active).
        /// True can be set to set the pin to high impedance (inactive).
        /// </summary>
        public bool InterruptReset
        {
            get
            {
                var intReset = Read8BitsFromRegister((byte) Register.SYSTEM_CONTROL);
                intReset = (byte)(intReset & (byte)Mask.INT_RESET);
                return Convert.ToBoolean(intReset);
            }
            set
            {
                var intReset = Read8BitsFromRegister((byte) Register.SYSTEM_CONTROL);
                intReset = (byte)(intReset & ((byte)Mask.INT_RESET ^ (byte)Mask.CLR));
                intReset = (byte)(intReset | Convert.ToByte(value) << 6);

                Write8BitsToRegister((byte) Register.SYSTEM_CONTROL, intReset);
            }
        }

        /// <summary>
        /// Gets or sets the currently set measurement time.
        /// </summary>
        public MeasurementTime MeasurementTime
        {
            get
            {
                var time = Read8BitsFromRegister((byte) Register.MODE_CONTROL1);
                time = (byte)(time & (byte)Mask.MEASUREMENT_TIME);
                return (MeasurementTime) time;
            }
            set
            {
                var time = Read8BitsFromRegister((byte) Register.MODE_CONTROL1);
                time = (byte)(time & ((byte)Mask.MEASUREMENT_TIME ^ (byte)Mask.CLR));
                time = (byte)(time | (byte)value);

                Write8BitsToRegister((byte) Register.MODE_CONTROL1, time);
            }
        }

        /// <summary>
        /// Gets whether the last measurement is valid.
        /// </summary>
        public bool MeasurementIsValid
        {
            get
            {
                var valid = Read8BitsFromRegister((byte)Register.MODE_CONTROL2);
                valid = (byte)(valid & (byte)Mask.VALID);
                return Convert.ToBoolean(valid);
            }
        }

        /// <summary>
        /// Gets or sets whether the measurement is active.
        /// </summary>
        public bool MeasurementIsActive
        {
            get
            {
                var active = Read8BitsFromRegister((byte)Register.MODE_CONTROL2);
                active = (byte)(active & (byte)Mask.RGBC_EN);
                return Convert.ToBoolean(active);
            }
            set
            {
                var active = Read8BitsFromRegister((byte)Register.MODE_CONTROL2);
                active = (byte)(active & ((byte)Mask.RGBC_EN ^ (byte)Mask.CLR));
                active = (byte)(active | (Convert.ToByte(value) << 4));

                Write8BitsToRegister((byte) Register.MODE_CONTROL2, active);
            }
        }

        /// <summary>
        /// Gets or sets the adc gain of the sensor.
        /// </summary>
        public AdcGain AdcGain
        {
            get
            {
                var adcGain = Read8BitsFromRegister((byte)Register.MODE_CONTROL2);
                adcGain = (byte)(adcGain & (byte)Mask.ADC_GAIN);
                return (AdcGain)adcGain;
            }
            set
            {
                var adcGain = Read8BitsFromRegister((byte) Register.MODE_CONTROL2);
                adcGain = (byte)(adcGain & ((byte)Mask.ADC_GAIN ^ (byte)Mask.CLR));
                adcGain = (byte)(adcGain | (byte)value);

                Write8BitsToRegister((byte) Register.MODE_CONTROL2, adcGain);
            }
        }

        /// <summary>
        /// Gets or sets the status of the interrupt signal.
        /// True indicates an active interrupt signal, false an inactive signal.
        /// </summary>
        public bool InterruptStatus
        {
            get
            {
                var intStatus = Read8BitsFromRegister((byte) Register.INTERRUPT);
                intStatus = (byte)(intStatus & (byte)Mask.INT_STATUS);
                return Convert.ToBoolean(intStatus);
            }
            set
            {
                var intStatus = Read8BitsFromRegister((byte)Register.INTERRUPT);
                intStatus = (byte)(intStatus & ((byte)Mask.INT_STATUS ^ (byte)Mask.CLR));
                intStatus = (byte)(intStatus | (Convert.ToByte(value) << 7));

                Write8BitsToRegister((byte) Register.INTERRUPT, intStatus);
            }
        }

        /// <summary>
        /// Gets or sets how the interrupt pin latches.
        /// False indicates that the interrupt pin is latched until interrupt register is read
        /// or initialized. True indicates that the pin is latched after each measurement.
        /// </summary>
        public bool InterruptLatch
        {
            get
            {
                var intLatch = Read8BitsFromRegister((byte)Register.INTERRUPT);
                intLatch = (byte)(intLatch & (byte)Mask.INT_LATCH);
                return Convert.ToBoolean(intLatch);
            }
            set
            {
                var intLatch = Read8BitsFromRegister((byte)Register.INTERRUPT);
                intLatch = (byte)(intLatch & ((byte)Mask.INT_LATCH ^ (byte) Mask.CLR));
                intLatch = (byte)(intLatch | (Convert.ToByte(value) << 4));

                Write8BitsToRegister((byte)Register.INTERRUPT, intLatch);
            }
        }

        /// <summary>
        /// Gets or sets the source channel of the interrupt.
        /// </summary>
        public InterruptSource InterruptSource
        {
            get
            {
                var intSource = Read8BitsFromRegister((byte)Register.INTERRUPT);
                intSource = (byte)(intSource & (byte)Mask.INT_SOURCE);
                return (InterruptSource)intSource;
            }
            set
            {
                var intLatch = Read8BitsFromRegister((byte)Register.INTERRUPT);
                intLatch = (byte)(intLatch & ((byte)Mask.INT_SOURCE ^ (byte)Mask.CLR));
                intLatch = (byte)(intLatch | ((byte)value << 2));

                Write8BitsToRegister((byte) Register.INTERRUPT, intLatch);
            }
        }

        /// <summary>
        /// Gets or sets whether the interrupt pin is enabled.
        /// </summary>
        public bool InterruptIsEnabled
        {
            get
            {
                var intPin = Read8BitsFromRegister((byte) Register.INTERRUPT);
                intPin = (byte) (intPin & (byte)Mask.INT_ENABLE);
                return Convert.ToBoolean(intPin);
            }
            set
            {
                var intPin = Read8BitsFromRegister((byte) Register.INTERRUPT);
                intPin = (byte)(intPin & ((byte)Mask.INT_ENABLE ^ (byte)Mask.CLR));
                intPin = (byte)(intPin | Convert.ToByte(value));

                Write8BitsToRegister((byte) Register.INTERRUPT, intPin);
            }
        }

        /// <summary>
        /// Gets or sets the persistence function of the interrupt.
        /// </summary>
        public InterruptPersistence InterruptPersistence
        {
            get
            {
                var intPersistence = Read8BitsFromRegister((byte)Register.PERSISTENCE);
                intPersistence = (byte)(intPersistence & (byte)Mask.PERSISTENCE);
                return (InterruptPersistence)intPersistence;
            }
            set
            {
                var intPersistence = Read8BitsFromRegister((byte) Register.PERSISTENCE);
                intPersistence = (byte)(intPersistence & ((byte)Mask.PERSISTENCE ^ (byte) Mask.CLR));
                intPersistence = (byte)(intPersistence | (byte)value);

                Write8BitsToRegister((byte)Register.PERSISTENCE, intPersistence);
            }
        }

        /// <summary>
        /// Gets the data in the red data register.
        /// </summary>
        public ushort RedDataRegister => Read16BitsFromRegister((byte)Register.RED_DATA);

        /// <summary>
        /// Gets the data in the green data register.
        /// </summary>
        public ushort GreenDataRegister => Read16BitsFromRegister((byte)Register.GREEN_DATA);

        /// <summary>
        /// Gets the data in the blue data register.
        /// </summary>
        public ushort BlueDataRegister => Read16BitsFromRegister((byte)Register.BLUE_DATA);

        /// <summary>
        /// Gets the data in the clear data register.
        /// </summary>
        public ushort ClearDataRegister => Read16BitsFromRegister((byte)Register.CLEAR_DATA);

        /// <summary>
        /// Gets the data in the dint data register. This register is used for internal calculation.
        /// </summary>
        public ushort DintDataRegister => Read16BitsFromRegister((byte)Register.DINT_DATA);

        /// <summary>
        /// Gets or sets the lower interrupt threshold.
        /// </summary>
        public ushort LowerInterruptThreshold
        {
            get => Read16BitsFromRegister((byte)Register.TL);
            set => WriteShortBitsToRegister((byte)Register.TL, value);
        }

        /// <summary>
        /// Gets or sets the higher interrupt threshold.
        /// </summary>
        public ushort HigherInterruptThreshold
        {
            get => Read16BitsFromRegister((byte)Register.TH);
            set => WriteShortBitsToRegister((byte)Register.TH, value);
        }

        public ChannelCompensationMultipliers ChannelCompensationMultipliers { get; set; }

        private I2cDevice _i2cDevice;
        private byte ManufacturerId => Read8BitsFromRegister((byte)Register.MANUFACTURER_ID);
        private byte PartId
        {
            get
            {
                var id = Read8BitsFromRegister((byte)Register.SYSTEM_CONTROL);
                return (byte) (id & (byte)Mask.PART_ID);
            }
        }

        public Bh1745(I2cDevice device)
        {
            _i2cDevice = device;
            ChannelCompensationMultipliers = new ChannelCompensationMultipliers
            {
                Red = 2.2,
                Green = 1.0,
                Blue = 1.8,
                Clear = 10.0
            };
        }

        /// <summary>
        /// Initializes the device.
        /// </summary>
        public void Init()
        {
            // check manufacturer and part Id
            if(ManufacturerId != 0xE0)
                throw new Exception($"Manufacturer ID {ManufacturerId} is not the same as expected 224. Please check if you are using the right device.");
            if(PartId != 0x0b)
                throw new Exception($"Part ID {PartId} is not the same as expected 11. Please check if you are using the right device.");

            // soft reset sensor
            // TODO: test how long sw reset takes
            var watch = new Stopwatch();
            
            SoftwareReset = true;
            watch.Start();
            while (SoftwareReset){}
            watch.Stop();

            // set measurement configuration
            InterruptReset = true;
            MeasurementTime = MeasurementTime.Ms160;
            AdcGain = AdcGain.X1;
            MeasurementIsActive = true;

            // set thresholds
            LowerInterruptThreshold = 0xFFFF;
            HigherInterruptThreshold = 0x0000;

            // set interrupt latch
            InterruptLatch = true;

            // TODO: sets the leds on pimoroni board?!?!
            InterruptIsEnabled = true;
            // TODO: need sleep here? pimoroni driver waits 320ms. For first measurement probably, so no!
            
            // write default value to Mode_Control3
            Write8BitsToRegister((byte)Register.MODE_CONTROL3, 0x02);
        }

        public Bh1745Color GetUncompensatedColor()
        {
            return new Bh1745Color(RedDataRegister, GreenDataRegister, BlueDataRegister, ClearDataRegister);
        }

        public Bh1745Color GetCompensatedColor()
        {
            var compensatedRed = RedDataRegister * ChannelCompensationMultipliers.Red;
            var compensatedGreen = GreenDataRegister * ChannelCompensationMultipliers.Green;
            var compensatedBlue = BlueDataRegister * ChannelCompensationMultipliers.Blue;
            var compensatedClear = ClearDataRegister * ChannelCompensationMultipliers.Clear;

            return new Bh1745Color(compensatedRed, compensatedGreen, compensatedBlue, compensatedClear);
        }

        private byte Read8BitsFromRegister(byte register)
        {
            _i2cDevice.WriteByte(register);
            var value = _i2cDevice.ReadByte();
            return value;
        }

        private ushort Read16BitsFromRegister(byte register)
        {
            Span<byte> bytes = stackalloc byte[2];

            _i2cDevice.WriteByte(register);
            _i2cDevice.Read(bytes);

            return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
        }

        // TODO: test!!!
        private void WriteShortBitsToRegister(byte register, ushort value)
        {
            var bytes = new byte[3];
            var source = new byte[2];

            // ensure order of bytes is little endian
            if (!BitConverter.IsLittleEndian)
                source = BitConverter.GetBytes(value).Reverse().ToArray();
            
            bytes[0] = register;
            Buffer.BlockCopy(source, 0, bytes, 1, source.Length);

            _i2cDevice.Write(bytes);
        }

        private void Write8BitsToRegister(byte register, byte data)
        {
            _i2cDevice.Write(new[] { register, data });
        }

        public void Dispose()
        {
            _i2cDevice?.Dispose();
            _i2cDevice = null;
        }
    }
}
