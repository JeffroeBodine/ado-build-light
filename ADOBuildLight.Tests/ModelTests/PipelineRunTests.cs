using ADOBuildLight.Models;
using FluentAssertions;
using NUnit.Framework;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class PipelineRunTests
    {
        [Test]
        public void PipelineRun_Properties_CanBeSet()
        {
            var pipelineRun = new PipelineRun();
            var id = 1;
            var name = "Run1";
            var state = "inProgress";
            var result = "none";
            var created = DateTime.Now;
            var finished = DateTime.Now.AddMinutes(10);
            var url = "http://example.com";

            pipelineRun.Id = id;
            pipelineRun.Name = name;
            pipelineRun.State = state;
            pipelineRun.Result = result;
            pipelineRun.CreatedDate = created;
            pipelineRun.FinishedDate = finished;
            pipelineRun.Url = url;

            pipelineRun.Id.Should().Be(id);
            pipelineRun.Name.Should().Be(name);
            pipelineRun.State.Should().Be(state);
            pipelineRun.Result.Should().Be(result);
            pipelineRun.CreatedDate.Should().Be(created);
            pipelineRun.FinishedDate.Should().Be(finished);
            pipelineRun.Url.Should().Be(url);
        }
    }
}
