using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using ADOBuildLight.Interfaces;
using ADOBuildLight.Models;

namespace ADOBuildLight.Services;

// Build status service implementation using Azure DevOps REST API
public class BuildStatusService : IBuildStatusService
{
  private readonly AzureDevOpsConfig _config;
  private readonly HttpClient _httpClient;

  public BuildStatusService(IConfiguration configuration)
  {
    _config = new AzureDevOpsConfig();
    var azureDevOpsSection = configuration.GetSection("AzureDevOps");
    _config.Organization = azureDevOpsSection["Organization"] ?? "";
    _config.Project = azureDevOpsSection["Project"] ?? "";
    _config.PipelineId = azureDevOpsSection["PipelineId"] ?? "";
    _config.PersonalAccessToken = azureDevOpsSection["PersonalAccessToken"] ?? "";

    _httpClient = new HttpClient();
    
    // Set up authentication header for Azure DevOps API
    var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_config.PersonalAccessToken}"));
    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
    _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
  }

  public async Task<PipelineRun?> GetLatestPipelineRunAsync()
  {
    try
    {
      // Construct the Azure DevOps REST API URL
      var apiUrl = $"https://dev.azure.com/{_config.Organization}/{_config.Project}/_apis/pipelines/{_config.PipelineId}/runs?api-version=7.1";
      
      Console.WriteLine($"Fetching pipeline status from: {apiUrl}");
      
      var response = await _httpClient.GetStringAsync(apiUrl);
      
      var pipelineRunsResponse = JsonSerializer.Deserialize<PipelineRunsResponse>(response);
      
      // Return the most recent pipeline run (first in the array)
      return pipelineRunsResponse?.Value?.FirstOrDefault();
    }
    catch (HttpRequestException ex)
    {
      Console.WriteLine($"HTTP error fetching pipeline status: {ex.Message}");
      throw;
    }
    catch (JsonException ex)
    {
      Console.WriteLine($"JSON parsing error: {ex.Message}");
      throw;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Unexpected error fetching pipeline status: {ex.Message}");
      throw;
    }
  }

  public void Dispose()
  {
    _httpClient?.Dispose();
  }
}
