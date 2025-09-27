namespace ADOBuildLight.Interfaces;

public interface IAppConfiguration
{
    string Organization { get; set; }
    string Project { get; set; }
    string PipelineId { get; set; }
    string PersonalAccessToken { get; set; }
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public List<string> DaysOfWeek { get; set; }
}