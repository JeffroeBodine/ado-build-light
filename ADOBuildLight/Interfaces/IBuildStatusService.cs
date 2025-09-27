using ADOBuildLight.Models;

namespace ADOBuildLight.Interfaces;

// Interface for build status monitoring
public interface IBuildStatusService
{
  Task<PipelineRun?> GetLatestPipelineRunAsync();
}
