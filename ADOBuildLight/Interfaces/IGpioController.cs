using System.Device.Gpio;

namespace ADOBuildLight.Interfaces
{
    public interface IGpioController : IDisposable
    {
        void OpenPin(int pinNumber, PinMode mode);
        void Write(int pinNumber, PinValue value);
    }
}
