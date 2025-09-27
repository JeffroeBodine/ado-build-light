using ADOBuildLight.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ADOBuildLight.Models;

public class Config : IConfig
{
    public string Organization { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string PipelineId { get; set; } = string.Empty;
    public string PersonalAccessToken { get; set; } = string.Empty;
    
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public List<string> DaysOfWeek { get; set; }

  public Config()
  {
    var configBuilder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

    var azureDevOpsSection = configBuilder.GetSection("AzureDevOps");
    var businessHoursSection = configBuilder.GetSection("BusinessHours");

    Organization = azureDevOpsSection["Organization"] ?? string.Empty;
    Project = azureDevOpsSection["Project"] ?? string.Empty;
    PipelineId = azureDevOpsSection["PipelineId"] ?? string.Empty;
    PersonalAccessToken = azureDevOpsSection["PersonalAccessToken"] ?? string.Empty;

    StartHour = int.Parse(businessHoursSection["StartHour"] ?? "0");
    EndHour = int.Parse(businessHoursSection["EndHour"] ?? "0");
    DaysOfWeek = businessHoursSection.GetSection("DaysOfWeek").Get<List<string>>() ?? new List<string>();
  }
}