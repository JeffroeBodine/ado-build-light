using System.Text.Json.Serialization;

namespace ADOBuildLight.Models;

public class PipelineRun
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public string? Result { get; set; }

    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }

    [JsonPropertyName("finishedDate")]
    public DateTime? FinishedDate { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

}
