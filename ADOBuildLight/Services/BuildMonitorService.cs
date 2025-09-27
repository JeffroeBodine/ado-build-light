using ADOBuildLight.Interfaces;

namespace ADOBuildLight.Services;

public class BuildMonitorService : IBuildMonitorService
{
    private readonly IBuildStatusService _buildStatusService;
    private readonly IGpioService _gpioService;

    public BuildMonitorService(IBuildStatusService buildStatusService, IGpioService gpioService)
    {
        _buildStatusService = buildStatusService;
        _gpioService = gpioService;
        _gpioService.Initialize();
    }

    public async Task RunMonitoringAsync(string badgeUrl)
    {
        // Note: badgeUrl parameter is now ignored since we use Azure DevOps REST API
        var latestRun = await _buildStatusService.GetLatestPipelineRunAsync();

        if (latestRun != null)
        {
            // Determine the overall status based on state and result
            string overallStatus = GetOverallStatus(latestRun.State, latestRun.Result);
            Console.WriteLine($"Pipeline: {latestRun.Name} (ID: {latestRun.Id})");
            Console.WriteLine($"State: {latestRun.State}, Result: {latestRun.Result ?? "N/A"}");
            Console.WriteLine($"Overall Status: {overallStatus}");
            
            HandleBuildStatus(overallStatus);
        }
        else
        {
            Console.WriteLine("No pipeline runs found");
            HandleBuildStatus("unknown");
        }
    }

    private string GetOverallStatus(string state, string? result)
    {
        // If pipeline is still in progress, return the state
        if (state.Equals("inProgress", StringComparison.OrdinalIgnoreCase))
        {
            return "inProgress";
        }
        
        // If pipeline is completed, return the result
        if (state.Equals("completed", StringComparison.OrdinalIgnoreCase))
        {
            return result?.ToLower() ?? "unknown";
        }
        
        // For other states, return the state
        return state.ToLower();
    }

    public void HandleBuildStatus(string status)
    {
        switch (status.ToLower())
        {
            case "succeeded":
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Build passed");
                _gpioService?.SetLightColor(GpioPins.Green);
                break;
            case "failed":
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("🚨 Build failed");
                _gpioService?.SetLightColor(GpioPins.Red);
                break;
            case "partiallysucceeded":
            case "partially succeeded":
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("⚠️ Build partially succeeded");
                _gpioService?.SetLightColor(GpioPins.Orange); // Using Orange pin
                break;
            case "inprogress":
            case "running":
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("🏃‍➡️ Build in progress");
                _gpioService?.SetLightColor(GpioPins.Yellow);
                break;
            case "canceled":
            case "cancelled":
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("🛑 Build cancelled");
                _gpioService?.SetLightColor(GpioPins.None);
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"❔ Build status unknown ({status})");
                _gpioService?.SetLightColor(GpioPins.None);
                break;
        }
        Console.ResetColor();
    }
}
