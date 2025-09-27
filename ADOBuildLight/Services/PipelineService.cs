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

    private readonly string _organization;
    private readonly string _project;
    private readonly string _pipelineId;
    private readonly string _personalAccessToken;

    public PipelineService(IConfig config)
    {
        _httpClient = new HttpClient();
        _organization = config.Organization;
        _project = config.Project;
        _pipelineId = config.PipelineId;
        _personalAccessToken = config.PersonalAccessToken;
    }

    public async Task<BuildResponse?> GetLatestPipelineRunAsync()
    {
        var url = $"https://dev.azure.com/{_organization}/{_project}/_apis/pipelines/{_pipelineId}/runs?api-version=7.1-preview.1";

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_personalAccessToken}")));

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var pipelineRuns = JsonSerializer.Deserialize<PipelineRunsResponse>(response);

            var mostRecentBuild = pipelineRuns?.Value.OrderByDescending(r => r.CreatedDate).FirstOrDefault();

            if (mostRecentBuild == null)
            {
                Console.Error.WriteLine("No pipeline runs found.");
                return null;
            }

            return await CheckBuildDetailsAsync(mostRecentBuild.Id);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching pipeline runs: {ex.Message}");
            return null;
        }
    }

    private async Task<BuildResponse?> CheckBuildDetailsAsync(int buildId)
    {
        var url = $"https://dev.azure.com/{_organization}/{_project}/_apis/build/builds/{buildId}?api-version=7.1";

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_personalAccessToken}")));

        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var buildResponse = JsonSerializer.Deserialize<BuildResponse>(response);
            //partiallySucceeded

            return buildResponse;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching build details: {ex.Message}");
            return null;
        }
    }
}