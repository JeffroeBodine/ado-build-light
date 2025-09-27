namespace ADOBuildLight.Interfaces;
public interface IConfig
{
    string Organization { get; set; }
    string Project { get; set; }
    string PipelineId { get; set; }
    string PersonalAccessToken { get; set; }
}