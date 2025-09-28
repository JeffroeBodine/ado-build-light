using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ADOBuildLight.Interfaces;
using ADOBuildLight.Models;

namespace ADOBuildLight.Services;

public class PipelineService : IPipelineService
{
    private readonly HttpClient _httpClient;
    private readonly IAppConfiguration _config;
    
    public PipelineService(IAppConfiguration config)
    {
        _httpClient = new HttpClient();
        _config = config;
    }

    public async Task<BuildResponse?> GetLatestPipelineRunAsync()
    {
        var url = $"https://dev.azure.com/{_config.AzureDevOps.Organization}/{_config.AzureDevOps.Project}/_apis/pipelines/{_config.AzureDevOps.PipelineId}/runs?api-version=7.1-preview.1";

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_config.AzureDevOps.PersonalAccessToken}")));

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
        var url = $"https://dev.azure.com/{_config.AzureDevOps.Organization}/{_config.AzureDevOps.Project}/_apis/build/builds/{buildId}?api-version=7.1";

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_config.AzureDevOps.PersonalAccessToken}")));

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