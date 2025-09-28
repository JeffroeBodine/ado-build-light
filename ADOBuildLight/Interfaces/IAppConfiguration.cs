using static ADOBuildLight.Models.AppConfiguration;

namespace ADOBuildLight.Interfaces;

public interface IAppConfiguration
{
    AzureDevOpsSettings AzureDevOps { get; }
    BusinessHoursSettings BusinessHours { get; }
}