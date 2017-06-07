using System.Threading.Tasks;

namespace Library
{
    public interface IPwmDriver
    {
        void DrivePercentage(byte servo, double percentage);
        bool IsDevicedInited { get; }
    }
}
