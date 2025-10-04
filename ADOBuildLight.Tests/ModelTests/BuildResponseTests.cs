using NUnit.Framework;
using ADOBuildLight.Models;
using FluentAssertions;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class BuildResponseTests
    {
        [TestCase("none")]
        [TestCase("succeeded")]
        [TestCase("partiallySucceeded")]
        [TestCase("failed")]
        [TestCase("canceled")]
        public void BuildResponse_Result_CanBeSet(string result)
        {
            var buildResponse = new BuildResponse();
            buildResponse.Result = result;

            buildResponse.Result.Should().Be(result);
        }

        [Test]
        public void BuildResponse_Result_CanBeSetToNull()
        {
            var buildResponse = new BuildResponse();
            buildResponse.Result = null;

            buildResponse.Result.Should().BeNull();
        }

        [TestCase("none")]
        [TestCase("inProgress")]
        [TestCase("completed")]
        [TestCase("canceling")]
        [TestCase("postponed")]
        [TestCase("notStarted")]
        [TestCase("all")]
        public void BuildResponse_Status_CanBeSet(string status)
        {
            var buildResponse = new BuildResponse();
            buildResponse.Status = status;

            buildResponse.Status.Should().Be(status);
        }
    }
}
