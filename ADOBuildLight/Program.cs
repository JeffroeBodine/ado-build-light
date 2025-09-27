using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;
using ADOBuildLight.Interfaces;
using ADOBuildLight.Services;

namespace ADOBuildLight;

class Program
{
  private static IConfiguration? _configuration;
  private static bool _wasAfterHours = false;

  static async Task Main(string[] args)
  {
    _configuration = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .Build();

    IGpioService? gpioService = null;
    try
    {
      // Centralized configuration validation
      var configurationService = new ConfigurationService(_configuration);
      var azureConfig = configurationService.GetValidatedAzureDevOpsConfig();
      if (azureConfig == null)
      {
        return; // Errors already printed by service
      }

      gpioService = CreateGpioService();
      var buildStatusService = new BuildStatusService(_configuration);
      var buildMonitorService = new BuildMonitorService(buildStatusService, gpioService);

      Console.WriteLine("Starting Azure DevOps pipeline monitoring... Press Ctrl+C to stop.");
      Console.WriteLine($"Organization: {azureConfig.Organization}");
      Console.WriteLine($"Project: {azureConfig.Project}");
      Console.WriteLine($"Pipeline ID: {azureConfig.PipelineId}");
      
      while (true)
      {

        
        await buildMonitorService.RunMonitoringAsync(""); // URL parameter is now ignored
        
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
      // Cleanup
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
