using ADOBuildLight.Interfaces;

namespace ADOBuildLight.Models;

public class Configuration : IAppConfiguration
{
    public string Organization { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string PipelineId { get; set; } = string.Empty;
    public string PersonalAccessToken { get; set; } = string.Empty;
    
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public List<string> DaysOfWeek { get; set; } = new List<string>();
}