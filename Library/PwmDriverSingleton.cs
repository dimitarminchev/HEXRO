using System.Threading.Tasks;

namespace Library
{
    public class PwmDriverSingleton
    {
        private IPwmDriver _driver;

        private object padlock = new object();
        private int failCounter = 0;

        public static IPwmDriver DriverInstance
        {
            get
            {
                return Instance._driver;
            }
        }

        private static PwmDriverSingleton _instance;
        public static PwmDriverSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PwmDriverSingleton();
                }
                return _instance;
            }
        }

        private PwmDriverSingleton()
        {
            Task.Factory.StartNew(async () =>
            {
                this._driver = await PwmDriver.Init();
            });
        }


    }
}
