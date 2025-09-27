using ADOBuildLight.Models;

namespace ADOBuildLight.Interfaces;

public interface IPipelineService
{
    Task<PipelineRun?> GetLatestPipelineRunAsync();
}
