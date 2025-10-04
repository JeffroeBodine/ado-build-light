using ADOBuildLight.Models;
using FluentAssertions;
using NUnit.Framework;

namespace ADOBuildLight.Tests.ModelTests
{
    [TestFixture]
    public class AppConfigurationTests
    {
        [Test]
        public void Default_Values_Are_Correct()
        {
            var config = new AppConfiguration();
            config.AzureDevOps.Should().NotBeNull();
            config.BusinessHours.Should().NotBeNull();
            config.AzureDevOps.Organization.Should().BeEmpty();
            config.AzureDevOps.Project.Should().BeEmpty();
            config.AzureDevOps.PipelineId.Should().BeEmpty();
            config.AzureDevOps.PersonalAccessToken.Should().BeEmpty();
            config.BusinessHours.StartHour.Should().Be(0);
            config.BusinessHours.EndHour.Should().Be(0);
            config.BusinessHours.DaysOfWeek.Should().NotBeNull();
            config.BusinessHours.DaysOfWeek.Should().BeEmpty();
        }

        [Test]
        public void Can_Set_And_Get_Properties()
        {
            var config = new AppConfiguration();
            config.AzureDevOps.Organization = "MyOrg";
            config.AzureDevOps.Project = "MyProject";
            config.AzureDevOps.PipelineId = "123";
            config.AzureDevOps.PersonalAccessToken = "token";
            config.BusinessHours.StartHour = 8;
            config.BusinessHours.EndHour = 17;
            config.BusinessHours.DaysOfWeek = new List<string> { "Monday", "Tuesday" };

            config.AzureDevOps.Organization.Should().Be("MyOrg");
            config.AzureDevOps.Project.Should().Be("MyProject");
            config.AzureDevOps.PipelineId.Should().Be("123");
            config.AzureDevOps.PersonalAccessToken.Should().Be("token");
            config.BusinessHours.StartHour.Should().Be(8);
            config.BusinessHours.EndHour.Should().Be(17);
            config.BusinessHours.DaysOfWeek.Should().Equal(new[] { "Monday", "Tuesday" });
        }
    }
}
