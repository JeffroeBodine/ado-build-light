using NUnit.Framework;
using ADOBuildLight.Models;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class PipelineRunTests
    {
        [Test]
        public void PipelineRun_Properties_CanBeSet()
        {
            // Arrange
            var pipelineRun = new PipelineRun();
            var id = 1;
            var name = "Run1";
            var state = "inProgress";
            var result = "none";
            var created = DateTime.Now;
            var finished = DateTime.Now.AddMinutes(10);
            var url = "http://example.com";

            // Act
            pipelineRun.Id = id;
            pipelineRun.Name = name;
            pipelineRun.State = state;
            pipelineRun.Result = result;
            pipelineRun.CreatedDate = created;
            pipelineRun.FinishedDate = finished;
            pipelineRun.Url = url;

            // Assert
            Assert.That(pipelineRun.Id, Is.EqualTo(id));
            Assert.That(pipelineRun.Name, Is.EqualTo(name));
            Assert.That(pipelineRun.State, Is.EqualTo(state));
            Assert.That(pipelineRun.Result, Is.EqualTo(result));
            Assert.That(pipelineRun.CreatedDate, Is.EqualTo(created));
            Assert.That(pipelineRun.FinishedDate, Is.EqualTo(finished));
            Assert.That(pipelineRun.Url, Is.EqualTo(url));
        }
    }
}
