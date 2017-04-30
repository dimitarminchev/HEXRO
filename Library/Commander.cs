using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Commander
    {

        /// <summary>
        ///  Moving One Servo
        /// </summary>
        /// <param name="servo">Servo Number [0..11]</param>
        /// <param name="percent">Percent [0..1]</param>
        public void Move(byte servo, double percent)
        {
            if (PwmDriverSingleton.Instance != null && PwmDriverSingleton.Instance.IsDevicedInited)
            {
                try
                {
                    PwmDriverSingleton.Instance.DrivePercentage(servo, percent);
                }
                catch {; ; } // On Error Do Nothing
            }
        }

        // Stop
        public void Stop()
        {
            if (PwmDriverSingleton.Instance != null && PwmDriverSingleton.Instance.IsDevicedInited)
                for (byte i = 0; i < 12; i++)
                    PwmDriverSingleton.Instance.DrivePercentage(i, 0.5);
        }

        // Walk
        public void Walk()
        {
            var cmd = new double[,]
            {
                //  0   1   2   3   4   5   6   7   8   9   10  11 
                {  .2, .8, .2, .8, .2, .5, .8, .8, .8, .5, .8, .5 },
                {  .8, .8, .8, .8, .8, .5, .2, .8, .2, .5, .2, .5 },
                {  .8, .5, .8, .5, .8, .8, .2, .5, .2, .8, .2, .8 },
                {  .2, .5, .2, .5, .2, .8, .8, .5, .8, .8, .8, .8 },
            };
            var rows = cmd.Length / 12;
            for (byte i = 0; i < rows; i++)
            {
                for (byte j = 0; j < 12; j++) Move(j, cmd[i, j]);
                Task.Delay(250).Wait();  // wait 1/4 seconds to complete movement
            }
        }

        // Rotate
        public void Rotate()
        {
            var cmd = new double[,]
            {
                //  0   1   2   3   4   5   6   7   8   9   10  11 
                {  .2, .8, .8, .5, .2, .8, .8, .5, .2, .8, .8, .5 },
                {  .8, .8, .2, .5, .8, .8, .2, .5, .8, .8, .2, .5 },
                {  .8, .5, .2, .8, .8, .5, .2, .8, .8, .5, .2, .8 },
                {  .2, .5, .8, .8, .2, .5, .8, .8, .2, .5, .8, .8 },
            };
            var rows = cmd.Length / 12;
            for (byte i = 0; i < rows; i++)
            {
                for (byte j = 0; j < 12; j++) Move(j, cmd[i, j]);
                Task.Delay(250).Wait();  // wait 1/4 seconds to complete movement
            }
        }

    }
}
