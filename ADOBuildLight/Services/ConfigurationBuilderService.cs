using ADOBuildLight.Models;
using Microsoft.Extensions.Configuration;

namespace ADOBuildLight.Services
{
    public static class ConfigurationBuilderService
    {
        public static AppConfiguration? LoadConfiguration(
            Func<AppConfiguration, bool> appSettingsValidation
        )
        {
            IConfigurationRoot configuration;

            if (!File.Exists("appsettings.json"))
                PromptUserToInputConfigurationValues();

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var config = new AppConfiguration();
            configuration.Bind(config);
            bool validAppSettingsValues = appSettingsValidation(config);
            if (!validAppSettingsValues)
            {
                return null;
            }

            return config;
        }

        private static void PromptUserToInputConfigurationValues()
        {
            Console.WriteLine("Configuration file 'appsettings.json' was not found.");
            Console.WriteLine(
                "Let's create one. Press Enter to accept defaults shown in [] where applicable.\n"
            );

            string Ask(string prompt, Func<string, bool>? validator = null)
            {
                while (true)
                {
                    Console.Write(prompt);
                    var value = Console.ReadLine() ?? string.Empty;
                    value = value.Trim();
                    if (validator != null && !validator(value))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        Console.WriteLine("Value cannot be empty. Try again.");
                        continue;
                    }
                    return value;
                }
            }

            string organization = Ask("Azure DevOps Organization name: ");
            string project = Ask("Project name: ");
            string pipelineId = Ask("Pipeline Id (numeric or GUID): ");
            string pat = Ask("Personal Access Token: ");

            // Business Hours defaults; keep simple for now.
            int startHour = 7;
            int endHour = 18;
            var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

            var configObject = new AppConfiguration
            {
                AzureDevOps = new AppConfiguration.AzureDevOpsSettings
                {
                    Organization = organization,
                    Project = project,
                    PipelineId = pipelineId,
                    PersonalAccessToken = pat,
                },
                BusinessHours = new AppConfiguration.BusinessHoursSettings
                {
                    StartHour = startHour,
                    EndHour = endHour,
                    DaysOfWeek = days.ToList(),
                },
            };

            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(
                    configObject,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
                );
                File.WriteAllText("appsettings.json", json);
                Console.WriteLine(
                    "Created 'appsettings.json'. Restart the application to use the new configuration."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write configuration file: {ex.Message}");
            }
        }
    }
}
