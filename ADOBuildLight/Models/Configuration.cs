using ADOBuildLight.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ADOBuildLight.Models;

public class Config : IConfig
{
    public string Organization { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string PipelineId { get; set; } = string.Empty;
    public string PersonalAccessToken { get; set; } = string.Empty;
    
    public Config()
    {
      var configBuilder = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .Build();

      var azureDevOpsSection = configBuilder.GetSection("AzureDevOps");
      Organization = azureDevOpsSection["Organization"] ?? string.Empty;
      Project = azureDevOpsSection["Project"] ?? string.Empty;
      PipelineId = azureDevOpsSection["PipelineId"] ?? string.Empty;
      PersonalAccessToken = azureDevOpsSection["PersonalAccessToken"] ?? string.Empty;
    }
}