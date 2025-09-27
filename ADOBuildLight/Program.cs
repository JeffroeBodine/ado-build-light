using System.Runtime.InteropServices;
using ADOBuildLight.Interfaces;
using ADOBuildLight.Services;
using ADOBuildLight.Models;

namespace ADOBuildLight;

class Program
{
  static async Task Main(string[] args)
  {
    var config = new Config();
    IGpioService? gpioService = null;
    try
    {






      gpioService = CreateGpioService();
      

      while (true)
      {
        Console.WriteLine($"Next check in 1 minute... ({DateTime.Now:HH:mm:ss})");
        await Task.Delay(TimeSpan.FromMinutes(1));
      }
    }
    catch (HttpRequestException ex)
    {
      Console.WriteLine($"Error fetching data: {ex.Message}");
    }
    finally
    {
      Console.ForegroundColor = ConsoleColor.White;
      gpioService?.Dispose();
    }
  }

  static IGpioService CreateGpioService()
  {
    try
    {
      if (IsRaspberryPi())
      {
        Console.WriteLine("Detected Raspberry Pi - using real GPIO");
        return new RealGpioService();
      }
      else
      {
        Console.WriteLine("Non-Raspberry Pi environment detected - using mock GPIO");
        return new MockGpioService();
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error creating GPIO service, falling back to mock: {ex.Message}");
      return new MockGpioService();
    }
  }

  static bool IsRaspberryPi()
  {
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      return false;
    try
    {
      return File.Exists("/proc/device-tree/model") &&
             File.ReadAllText("/proc/device-tree/model").Contains("Raspberry Pi");
    }
    catch
    {
      return false;
    }
  }
}
