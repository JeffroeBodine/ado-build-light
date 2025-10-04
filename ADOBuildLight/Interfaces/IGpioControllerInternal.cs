using System.Device.Gpio;

namespace ADOBuildLight.Interfaces
{
    public interface IGpioControllerInternal
    {
        void OpenPin(int pinNumber, PinMode mode);
        void Write(int pinNumber, PinValue value);
        void Dispose();
    }
}
