using System.Runtime.InteropServices;
using ADOBuildLight.Interfaces;
using ADOBuildLight.Services;
using Microsoft.Extensions.Configuration;

namespace ADOBuildLight;

class Program
{
  static async Task Main(string[] args)
  {
    var configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .Build();

    var config = new Models.AppConfiguration();
    configuration.Bind(config);

    IPipelineService pipelineService = new PipelineService(config);
    IGpioService? gpioService = CreateGpioService();
    gpioService.Initialize();

    try
    {
      while (true)
      {
        var latestRun = await pipelineService.GetLatestPipelineRunAsync();

        if (IsWithinBusinessHours(config))
        {
          var overallStatus = GetOverallStatus(latestRun?.Status, latestRun?.Result);
          Console.WriteLine($"Overall Status: {overallStatus}");

          UpdateBuildLight(overallStatus, gpioService);
        }
        else
        { 
          Console.WriteLine($"Outside of business hours. Skipping check. ({DateTime.Now:HH:mm:ss})");
          // Optionally turn lights off or set a specific color for "off-duty"
          UpdateBuildLight("offduty", gpioService);
        }

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
        return new RealGpioService(new GpioControllerWrapper());
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

  static string GetOverallStatus(string? state, string? result)
  {
    // If pipeline is still in progress, return the state
    if (state != null && state.Equals("inProgress", StringComparison.OrdinalIgnoreCase))
    {
      return "inProgress";
    }

    // If pipeline is completed, return the result
    if (state != null && state.Equals("completed", StringComparison.OrdinalIgnoreCase))
    {
      return result?.ToLower() ?? "unknown";
    }

    // For other states, return the state or "unknown" if null
    return state?.ToLower() ?? "unknown";
  }
  static void UpdateBuildLight(string status, IGpioService? gpioService)
  {
    switch (status.ToLower())
    {
      case "succeeded":
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Build passed");
        gpioService?.SetLightColor(GpioPins.Green);
        break;
      case "failed":
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("🚨 Build failed");
        gpioService?.SetLightColor(GpioPins.Red);
        break;
      case "partiallysucceeded":
      case "partially succeeded":
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine("⚠️ Build partially succeeded");
        gpioService?.SetLightColor(GpioPins.Orange); // Using Orange pin
        break;
      case "inprogress":
      case "running":
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("🏃‍➡️ Build in progress");
        gpioService?.SetLightColor(GpioPins.Yellow);
        break;
      case "canceled":
      case "cancelled":
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("🛑 Build cancelled");
        gpioService?.SetLightColor(GpioPins.Red);
        break;
      case "offduty":
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("💤 Off duty. Lights out.");
        gpioService?.SetLightColor(GpioPins.None);
        break;
      default:
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"❔ Build status unknown ({status})");
        gpioService?.SetLightColor(GpioPins.None);
        break;
    }
    Console.ResetColor();
  }

  static bool IsWithinBusinessHours(IAppConfiguration config)
  {
    var now = DateTime.Now;

    if (config.BusinessHours.DaysOfWeek == null || !config.BusinessHours.DaysOfWeek.Any())
      return true; // No restrictions if not configured

    if (!config.BusinessHours.DaysOfWeek.Contains(now.DayOfWeek.ToString(), StringComparer.OrdinalIgnoreCase))
      return false;

    if (now.Hour < config.BusinessHours.StartHour || now.Hour >= config.BusinessHours.EndHour)
      return false;
    
    return true;
  }
}
