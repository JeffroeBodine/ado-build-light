namespace ADOBuildLight.Interfaces;

// Interface for build monitoring service
public interface IBuildMonitorService
{
  Task RunMonitoringAsync(string badgeUrl);
}
