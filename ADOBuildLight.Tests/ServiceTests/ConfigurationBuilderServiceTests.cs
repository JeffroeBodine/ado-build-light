using System.Text;
using ADOBuildLight.Models;
using ADOBuildLight.Services;
using FluentAssertions;
using NUnit.Framework;

namespace ADOBuildLight.Tests.ServiceTests;

[TestFixture]
public class ConfigurationBuilderServiceTests
{
    private string _originalDirectory = string.Empty!;

    [SetUp]
    public void SetUp()
    {
        _originalDirectory = Directory.GetCurrentDirectory();
    }

    [TearDown]
    public void TearDown()
    {
        Directory.SetCurrentDirectory(_originalDirectory);
    }

    [Test]
    public void LoadConfiguration_WithValidAppSettings_ReturnsBoundConfiguration()
    {
        // The test project links appsettings.test.json as appsettings.json to output dir.
        // Arrange & Act
        var config = ConfigurationBuilderService.LoadConfiguration(_ => true);

        // Assert
        config.Should().NotBeNull();
        config!.AzureDevOps.Organization.Should().Be("test-org");
        config.AzureDevOps.Project.Should().Be("test-project");
        config.AzureDevOps.PipelineId.Should().Be("123");
        config.AzureDevOps.PersonalAccessToken.Should().Be("test-pat");
        config.BusinessHours.StartHour.Should().Be(9);
        config.BusinessHours.EndHour.Should().Be(17);
        config.BusinessHours.DaysOfWeek.Should().Contain(new[] { "Monday", "Friday" });
    }

    [Test]
    public void LoadConfiguration_WhenValidationFails_ReturnsNull()
    {
        // Arrange & Act
        var config = ConfigurationBuilderService.LoadConfiguration(_ => false);

        // Assert
        config.Should().BeNull();
    }

    [Test]
    public void LoadConfiguration_WhenFileMissing_PromptsAndCreatesFile()
    {
        // We simulate missing file by working in a temp directory with no appsettings.json
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        Directory.SetCurrentDirectory(tempDir);

        // Provide console input values for interactive prompts. Each Console.ReadLine() returns next line.
        // Prompts ask for: organization, project, pipeline id, pat.
        var inputLines = new[] { "org-from-input", "proj-from-input", "456", "pat-value" };
        Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputLines)));

        var sb = new StringBuilder();
        var writer = new StringWriter(sb);
        Console.SetOut(writer);

        // Act
        var config = ConfigurationBuilderService.LoadConfiguration(_ => true);

        // Flush output
        writer.Flush();

        // Assert: file created
        File.Exists(Path.Combine(tempDir, "appsettings.json")).Should().BeTrue();
        config.Should().NotBeNull();
        config!.AzureDevOps.Organization.Should().Be("org-from-input");
        config.AzureDevOps.Project.Should().Be("proj-from-input");
        config.AzureDevOps.PipelineId.Should().Be("456");
        config.AzureDevOps.PersonalAccessToken.Should().Be("pat-value");
        // Business hours defaulted in prompt method
        config.BusinessHours.StartHour.Should().Be(7);
        config.BusinessHours.EndHour.Should().Be(18);
        config
            .BusinessHours.DaysOfWeek.Should()
            .BeEquivalentTo(new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" });

        // Basic sanity check that prompt text was written
        sb.ToString().Should().Contain("Configuration file 'appsettings.json' was not found.");
    }
}
