using NUnit.Framework;
using ADOBuildLight.Models;
using System.Collections.Generic;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class PipelineRunResponseTests
    {
        [Test]
        public void PipelineRunsResponse_Properties_CanBeSet()
        {
            // Arrange
            var response = new PipelineRunsResponse();
            var count = 1;
            var value = new List<PipelineRun> { new PipelineRun() };

            // Act
            response.Count = count;
            response.Value = value;

            // Assert
            Assert.That(response.Count, Is.EqualTo(count));
            Assert.That(response.Value, Is.EqualTo(value));
        }
    }
}
