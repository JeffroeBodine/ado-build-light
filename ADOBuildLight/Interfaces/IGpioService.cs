using System.Device.Gpio;

namespace ADOBuildLight.Interfaces;

// Interface for GPIO operations
public interface IGpioService
{
    void Initialize();
    void SetLightColor(int pin);
    void Dispose();
}
