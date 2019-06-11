using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;

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

        private byte ManufacturerId => Read8BitsFromRegister((byte)Register.MANUFACTURER_ID);
        private byte PartId
        {
            get
            {
                var id = Read8BitsFromRegister((byte)Register.SYSTEM_CONTROL);
                return (byte) (id & (byte)Mask.PartId);
            }
        }
        private I2cDevice _i2cDevice;

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

        }

        /// <summary>
        /// Triggers a software reset on the device. All registers are reset and the sensor goes to power down mode. 
        /// </summary>
        public void SoftReset()
        {

            Write8BitsToRegister((byte)Register.MODE_CONTROL1, );
        }

        private byte Read8BitsFromRegister(byte register)
        {
            _i2cDevice.WriteByte(register);
            var value = _i2cDevice.ReadByte();
            return value;
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
