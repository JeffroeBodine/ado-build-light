using ADOBuildLight.Models;

namespace ADOBuildLight.Interfaces;

public interface IPipelineService
{
    Task<BuildResponse?> GetLatestPipelineRunAsync();
}
