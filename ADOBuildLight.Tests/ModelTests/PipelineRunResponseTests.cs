using ADOBuildLight.Models;
using FluentAssertions;
using NUnit.Framework;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class PipelineRunResponseTests
    {
        [Test]
        public void PipelineRunsResponse_Properties_CanBeSet()
        {
            var response = new PipelineRunsResponse();
            var count = 1;
            var value = new List<PipelineRun> { new PipelineRun() };

            response.Count = count;
            response.Value = value;

            response.Count.Should().Be(count);
            response.Value.Should().Equal(value);
        }
    }
}
