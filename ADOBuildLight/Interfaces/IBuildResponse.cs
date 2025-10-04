using System.Text.Json.Serialization;

namespace ADOBuildLight.Models;

public interface IBuildResponse
{
    [JsonPropertyName("status")]
    string Status { get; set; }

    [JsonPropertyName("result")]
    string? Result { get; set; }
}
