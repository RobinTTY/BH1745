// TODO: clarify API surface
// TODO: headers, examples

using System;
using System.Buffers.Binary;
using System.Device.I2c;

namespace BH1745Driver
{
    public class Bh1745 : IDisposable
    {
        /// <summary>
        /// The primary I2c address of the BH1745
        /// </summary>
        public byte PrimaryDeviceI2cAddress = 0x38;

        /// <summary>
        /// The secondary I2c address of the BH1745
        /// </summary>
        public byte SecondaryDeviceI2cAddress = 0x39;

        public bool MeasurementIsValid
        {
            get
            {
                var valid = Read8BitsFromRegister((byte) Register.MODE_CONTROL2);
                valid = (byte)(valid & (byte) Mask.VALID);
                return Convert.ToBoolean(valid);
            }
        }

        public bool MeasurementIsActive
        {
            get
            {
                var active = Read8BitsFromRegister((byte) Register.MODE_CONTROL2);
                active = (byte) (active & (byte)Mask.RGBC_EN);
                return Convert.ToBoolean(active);
            }
            set
            {
                var active = Read8BitsFromRegister((byte) Register.MODE_CONTROL2);
                active = (byte)(active & ((byte)Mask.RGBC_EN ^ (byte) Mask.CLR));
                active = (byte)(active | Convert.ToByte(value));

                Write8BitsToRegister((byte) Register.MODE_CONTROL2, active);
            }
        }

        public MeasurementTime MeasurementTime
        {
            get
            {
                var time = Read8BitsFromRegister((byte) Register.MODE_CONTROL1);
                time = (byte)(time & (byte)Mask.MEASUREMENT_TIME);
                return (MeasurementTime)time;
            }
            set
            {
                var filter = Read8BitsFromRegister((byte)Register.MODE_CONTROL1);
                filter = (byte)(filter & ((byte)Mask.MEASUREMENT_TIME ^ (byte) Mask.CLR));
                filter = (byte)(filter | (byte)value);

                Write8BitsToRegister((byte)Register.MODE_CONTROL1, filter);
            }
        }

        public ushort RedDataRegister => Read16BitsFromRegister((byte) Register.RED_DATA);
        public ushort GreenDataRegister => Read16BitsFromRegister((byte) Register.GREEN_DATA);
        public ushort BlueDataRegister => Read16BitsFromRegister((byte) Register.BLUE_DATA);
        public ushort ClearDataRegister => Read16BitsFromRegister((byte) Register.CLEAR_DATA);
        public ushort DintDataRegister => Read16BitsFromRegister((byte) Register.DINT_DATA);
        
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

            // set default configuration
            MeasurementTime = MeasurementTime.Ms160;
            MeasurementIsActive = true;

            // write default value to Mode_Control3
            Write8BitsToRegister((byte)Register.MODE_CONTROL3, 0x02);
        }

        /// <summary>
        /// Triggers a software reset on the device. All registers are reset and the sensor goes to power down mode. 
        /// </summary>
        public void TriggerSoftReset()
        {
            var status = Read8BitsFromRegister((byte)Register.SYSTEM_CONTROL);
            status = (byte)(status & ((byte)Mask.SW_RESET ^ (byte)Mask.CLR));
            status = (byte)(status | 0x01 << 7);

            Write8BitsToRegister((byte)Register.SYSTEM_CONTROL, status);
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
