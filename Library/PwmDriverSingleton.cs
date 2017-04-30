using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class PwmDriverSingleton
    {
        // Private Part
        private PwmDriver _driver;
        private static PwmDriverSingleton _instance;
        private PwmDriverSingleton()
        {
            Task.Factory.StartNew(async () =>
            {
                this._driver = await PwmDriver.Init();
            });
        }

        // Public Part
        public static PwmDriver Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PwmDriverSingleton();
                }
                return _instance._driver;
            }
        }
    }
}
