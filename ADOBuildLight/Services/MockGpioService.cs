using ADOBuildLight.Interfaces;

namespace ADOBuildLight.Services;

// Mock GPIO implementation for non-Raspberry Pi environments
public class MockGpioService : IGpioService
{
  public void Initialize()
  {
    Console.WriteLine("[MOCK GPIO] Initializing GPIO pins (simulation mode)");
    
    // Simulate test sequence
    SetLightColor(GpioPins.None);
    Thread.Sleep(100); // Shorter delays for mock
    SetLightColor(GpioPins.Red);
    Thread.Sleep(100);
    SetLightColor(GpioPins.Yellow);
    Thread.Sleep(100);
    SetLightColor(GpioPins.Green);
    Thread.Sleep(100);
  }

  public void SetLightColor(int color)
  {
    var colorName = color switch
    {
      var pin when pin == GpioPins.Red => "RED",
      var pin when pin == GpioPins.Yellow => "YELLOW", 
      var pin when pin == GpioPins.Green => "GREEN",
      var pin when pin == GpioPins.Orange => "ORANGE",
      _ => "OFF"
    };
    Console.WriteLine($"[MOCK GPIO] Setting light to: {colorName}");
  }

  public void Dispose()
  {
    Console.WriteLine("[MOCK GPIO] Disposing GPIO controller (simulation mode)");
  }
}
