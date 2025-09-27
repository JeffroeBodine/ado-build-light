using NUnit.Framework;
using ADOBuildLight.Models;
using System.Collections.Generic;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void Configuration_Properties_CanBeSet()
        {
            // Arrange
            var config = new Configuration();
            var org = "MyOrg";
            var project = "MyProject";
            var pipelineId = "123";
            var pat = "MyPAT";
            var startHour = 8;
            var endHour = 18;
            var days = new List<string> { "Wednesday" };

            // Act
            config.Organization = org;
            config.Project = project;
            config.PipelineId = pipelineId;
            config.PersonalAccessToken = pat;
            config.StartHour = startHour;
            config.EndHour = endHour;
            config.DaysOfWeek = days;

            // Assert
            Assert.That(config.Organization, Is.EqualTo(org));
            Assert.That(config.Project, Is.EqualTo(project));
            Assert.That(config.PipelineId, Is.EqualTo(pipelineId));
            Assert.That(config.PersonalAccessToken, Is.EqualTo(pat));
            Assert.That(config.StartHour, Is.EqualTo(startHour));
            Assert.That(config.EndHour, Is.EqualTo(endHour));
            Assert.That(config.DaysOfWeek, Is.EqualTo(days));
        }
    }
}
