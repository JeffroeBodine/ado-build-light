using ADOBuildLight.Interfaces;
using ADOBuildLight.Models;
using Microsoft.Extensions.Configuration;

namespace ADOBuildLight.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AzureDevOpsConfig? GetValidatedAzureDevOpsConfig()
    {
        var section = _configuration.GetSection("AzureDevOps");
        var organization = section["Organization"];
        var project = section["Project"];
        var pipelineId = section["PipelineId"];
        var pat = section["PersonalAccessToken"];

        if (string.IsNullOrWhiteSpace(organization) ||
            string.IsNullOrWhiteSpace(project) ||
            string.IsNullOrWhiteSpace(pipelineId) ||
            string.IsNullOrWhiteSpace(pat))
        {
            WriteError(
                "Azure DevOps configuration is missing or incomplete in appsettings.json.",
                "Please ensure Organization, Project, PipelineId, and PersonalAccessToken are all configured.",
                "Example:",
                """
                {
                  \"AzureDevOps\": {
                    \"Organization\": \"your-org\",
                    \"Project\": \"your-project\", 
                    \"PipelineId\": \"123\",
                    \"PersonalAccessToken\": \"your-pat-token\"
                  }
                }
                """
            );
            return null;
        }

        if (pat == "YOUR_PAT_TOKEN_HERE")
        {
            WriteError("Please replace 'YOUR_PAT_TOKEN_HERE' with your actual Azure DevOps Personal Access Token in appsettings.json");
            return null;
        }

        return new AzureDevOpsConfig
        {
            Organization = organization!,
            Project = project!,
            PipelineId = pipelineId!,
            PersonalAccessToken = pat!
        };
    }

    private static void WriteError(params string[] lines)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        foreach (var l in lines) Console.WriteLine(l);
        Console.ForegroundColor = original;
    }
}
