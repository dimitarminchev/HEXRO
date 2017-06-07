using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Library
{
    public class PwmDriver : IPwmDriver
    {
        // Private part
        private const byte RegMode1 = 0x0;
        private const byte RegPrescale = 0xFE;
        private const double ClockFrequency = 25000000;
        private const byte Servo0OnL = 0x6;
        private const byte Servo0OnH = 0x7;
        private const byte Servo0OffL = 0x8;
        private const byte Servo0OffH = 0x9;
        private const int I2CResetAddress = 0x0;
        private const ushort PulseResolution = 4096;
        private static readonly byte[] ResetCommand = { 0x06 };
        private static readonly uint MinPulse = 150;
        private static readonly uint MaxPulse = 450;
        private static volatile bool _isInited;
        private static I2cDevice _primaryDevice;
        private static I2cDevice _resetDevice;

        // Public part
        public static string I2CControllerName = "I2C1";
        public static byte PwmI2CAddr = 0x40;
        public static int PwmFreq = 50;
        public bool IsDevicedInited => _isInited;

        /// <summary>
        /// PWM Driver Constructor
        /// </summary>
        /// <param name="i2CAddress">I2C Device Address</param>
        /// <param name="pwmFreq">Pulse Width Modulation Frequency</param>
        /// <param name="controllerName">Device Controller Name</param>
        private PwmDriver(byte i2CAddress = 0x40, int pwmFreq = 50, string controllerName = "I2C1")
        {
            PwmI2CAddr = i2CAddress;
            I2CControllerName = controllerName;
            PwmFreq = pwmFreq;
        }

        /// <summary>
        /// PWM Driver Initialization
        /// </summary>
        /// <param name="i2CAddress">I2C Device Address</param>
        /// <param name="pwmFreq">Pulse Width Modulation Frequency</param>
        /// <param name="controllerName">Device Controller Name</param>
        /// <returns></returns>
        public static async Task<IPwmDriver> Init(byte i2CAddress = 0x40, int pwmFreq = 50, string controllerName = "I2C1")
        {
            var pwmDriver = new PwmDriver(i2CAddress, pwmFreq, controllerName);
            await EnsureInitializedAsync();
            return pwmDriver;
        }

        /// <summary>
        /// Drive Single Servo by Percent
        /// </summary>
        /// <param name="servo">Servo Number</param>
        /// <param name="percentage">Percent [0..1]</param>
        public void DrivePercentage(byte servo, double percentage)
        {
            if (percentage > 1.0) percentage = 1.0;
            if (percentage < 0) percentage = 0;
            var intPercentage = (int)(percentage * 100);
            var pulse = Map(intPercentage, 0, 100, MinPulse, MaxPulse);
            if (intPercentage == 0) pulse = 0;

            Pulse(servo, 0, (int)pulse);
        }

        /// <summary>
        /// Create Pulse Width Modulation
        /// </summary>
        /// <param name="num">Servo Number</param>
        /// <param name="on">On</param>
        /// <param name="off">Off</param>
        public void Pulse(byte num, int on, int off)
        {
            if (!_isInited) return;
            Write8(RegMode1, 0x0);
            Write8((byte)(Servo0OnL + 4 * num), (byte)on);
            Write8((byte)(Servo0OnH + 4 * num), (byte)(on >> 8));
            Write8((byte)(Servo0OffL + 4 * num), (byte)off);
            Write8((byte)(Servo0OffH + 4 * num), (byte)(off >> 8));
        }

        private static async Task EnsureInitializedAsync()
        {
            // If already initialized, done
            if (_isInited) return;

            // Validate
            if (string.IsNullOrWhiteSpace(I2CControllerName)) throw new Exception("Controller name not set");

            // Get a query for I2C
            var aqs = I2cDevice.GetDeviceSelector(I2CControllerName);

            // Find the first I2C device
            var di = (await DeviceInformation.FindAllAsync(aqs)).FirstOrDefault();
            
            // Make sure we found an I2C device
            if (di == null) throw new Exception("Device Info null: " + I2CControllerName);

            // Connection settings for primary device
            var primarySettings = new I2cConnectionSettings(PwmI2CAddr)
            {
                BusSpeed = I2cBusSpeed.FastMode,
                SharingMode = I2cSharingMode.Exclusive
            };

            // Get the primary device
            _primaryDevice = await I2cDevice.FromIdAsync(di.Id, primarySettings);
            if (_primaryDevice == null) throw new Exception("PCA9685 primary device not found");

            // Connection settings for reset device
            var resetSettings = new I2cConnectionSettings(PwmI2CAddr);
            resetSettings.SlaveAddress = I2CResetAddress;

            // Get the reset device
            _resetDevice = await I2cDevice.FromIdAsync(di.Id, resetSettings);
            if (_resetDevice == null) throw new Exception("PCA9685 reset device not found");

            // Initialize the controller
            await InitializeControllerAsync();

            // Done initializing
            _isInited = true;
        }

        private static async Task InitializeControllerAsync()
        {
            if (_primaryDevice == null) return;

            ResetController();
            var prescaleval = ClockFrequency;
            prescaleval /= PulseResolution;
            prescaleval /= PwmFreq;
            prescaleval -= 1;
            var prescale = (byte)Math.Floor(prescaleval + 0.5);
            Write8(RegPrescale, prescale);

            await RestartControllerAsync(0xA1);
        }

        private static void ResetController()
        {
            _resetDevice.Write(ResetCommand);
        }

        private static async Task RestartControllerAsync(byte mode1)
        {
            Write8(RegMode1, mode1);

            // Wait for more than 500us to stabilize.  	
            await Task.Delay(1);
        }

        private byte Read8(byte addr)
        {
            var readBuffer = new byte[1];
            _primaryDevice.WriteRead(new[] { addr }, readBuffer);
            return readBuffer[0];
        }

        private static void Write8(byte addr, byte d)
        {
            _primaryDevice.Write(new[] { addr, d });
        }

        private long Map(long x, long in_min, long in_max, long out_min, long out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

    }
}
