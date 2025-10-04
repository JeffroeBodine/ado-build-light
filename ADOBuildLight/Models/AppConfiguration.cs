using ADOBuildLight.Interfaces;

namespace ADOBuildLight.Models;

public class AppConfiguration : IAppConfiguration
{
    public class AzureDevOpsSettings
    {
        public string Organization { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;
        public string PipelineId { get; set; } = string.Empty;
        public string PersonalAccessToken { get; set; } = string.Empty;
    }

    public class BusinessHoursSettings
    {
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public List<string> DaysOfWeek { get; set; } = new List<string>();
    }

    public AzureDevOpsSettings AzureDevOps { get; set; } = new();
    public BusinessHoursSettings BusinessHours { get; set; } = new();
}
