using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ADOBuildLight.Interfaces;
using ADOBuildLight.Models;
using Microsoft.Extensions.Logging;

namespace ADOBuildLight.Services;

public class PipelineService : IPipelineService
{
    private readonly HttpClient _httpClient;
    private readonly IConfig _config;

    public PipelineService(IConfig config)
    {
        _httpClient = new HttpClient();
        _config = config;
    }

    public async Task<PipelineRun?> GetLatestPipelineRunAsync()
    {
        var organization = _config.Organization;
        var project = _config.Project;
        var pipelineID = _config.PipelineId;
        var pat = _config.PersonalAccessToken;

        var url = $"https://dev.azure.com/{organization}/{project}/_apis/pipelines/{pipelineID}/runs?api-version=7.1-preview.1";

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var pipelineRuns = JsonSerializer.Deserialize<PipelineRunsResponse>(response);

            var mostRecentBuild = pipelineRuns?.Value.OrderByDescending(r => r.CreatedDate).FirstOrDefault();
            return mostRecentBuild;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching pipeline runs: {ex.Message}");
            return null;
        }
    }
}
