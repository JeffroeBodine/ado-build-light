using System.Device.Gpio;
using ADOBuildLight.Interfaces;

namespace ADOBuildLight.Services;

// Real GPIO implementation
public class RealGpioService : IGpioService
{
    private readonly IGpioController _controller;

    // Allows tests to skip the startup delay sequence for faster execution
    public static bool SkipStartupDelays { get; set; } = false;

    public RealGpioService(IGpioController controller)
    {
        _controller = controller;
    }

    public void Initialize()
    {
        _controller.OpenPin(GpioPins.Red, PinMode.Output);
        _controller.OpenPin(GpioPins.Yellow, PinMode.Output);
        _controller.OpenPin(GpioPins.Green, PinMode.Output);

        // Test sequence
        SetLightColor(GpioPins.None);
        Sleep(500);
        SetLightColor(GpioPins.Red);
        Sleep(500);
        SetLightColor(GpioPins.Yellow);
        Sleep(500);
        SetLightColor(GpioPins.Green);
        Sleep(500);
        _controller.Write(GpioPins.Green, PinValue.High);
    }

    private static void Sleep(int ms)
    {
        if (SkipStartupDelays)
            return;
        Thread.Sleep(ms);
    }

    public void SetLightColor(int color)
    {
        switch (color)
        {
            case var pin when pin == GpioPins.Red:
                _controller.Write(GpioPins.Red, PinValue.Low);
                _controller.Write(GpioPins.Yellow, PinValue.High);
                _controller.Write(GpioPins.Green, PinValue.High);
                break;
            case var pin when pin == GpioPins.Yellow:
                _controller.Write(GpioPins.Red, PinValue.High);
                _controller.Write(GpioPins.Yellow, PinValue.Low);
                _controller.Write(GpioPins.Green, PinValue.High);
                break;
            case var pin when pin == GpioPins.Green:
                _controller.Write(GpioPins.Red, PinValue.High);
                _controller.Write(GpioPins.Yellow, PinValue.High);
                _controller.Write(GpioPins.Green, PinValue.Low);
                break;
            case var pin when pin == GpioPins.Orange:
                _controller.Write(GpioPins.Red, PinValue.Low);
                _controller.Write(GpioPins.Yellow, PinValue.Low);
                _controller.Write(GpioPins.Green, PinValue.High);
                break;
            default:
                _controller.Write(GpioPins.Red, PinValue.High);
                _controller.Write(GpioPins.Yellow, PinValue.High);
                _controller.Write(GpioPins.Green, PinValue.High);
                break;
        }
    }

    public void Dispose()
    {
        _controller?.Dispose();
    }
}
