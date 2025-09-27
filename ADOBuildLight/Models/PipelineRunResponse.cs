using System.Text.Json.Serialization;

namespace ADOBuildLight.Models;

public class PipelineRunsResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("value")]
    public List<PipelineRun> Value { get; set; } = new();
}