using System.Text.Json.Serialization;

namespace ADOBuildLight.Models;

public class BuildResponse : IBuildResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public string? Result { get; set; }
}
