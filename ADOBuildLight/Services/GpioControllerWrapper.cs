using System.Device.Gpio;
using ADOBuildLight.Interfaces;

namespace ADOBuildLight.Services
{
    public class GpioControllerWrapper : IGpioController
    {
        private readonly GpioController _controller;

        public GpioControllerWrapper()
        {
            _controller = new GpioController();
        }

        public void OpenPin(int pinNumber, PinMode mode)
        {
            _controller.OpenPin(pinNumber, mode);
        }

        public void Write(int pinNumber, PinValue value)
        {
            _controller.Write(pinNumber, value);
        }

        public void Dispose()
        {
            _controller.Dispose();
        }
    }
}
