using ADOBuildLight.Models;

namespace ADOBuildLight.Interfaces;

public interface IConfigurationService
{
    /// <summary>
    /// Loads and validates the Azure DevOps configuration.
    /// Returns a populated AzureDevOpsConfig if valid; otherwise null (errors already written to console).
    /// </summary>
    AzureDevOpsConfig? GetValidatedAzureDevOpsConfig();
}
