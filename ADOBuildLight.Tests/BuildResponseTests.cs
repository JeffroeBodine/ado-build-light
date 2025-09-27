using NUnit.Framework;
using ADOBuildLight.Models;

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
            // Arrange
            var buildResponse = new BuildResponse();

            // Act
            buildResponse.Result = result;

            // Assert
            Assert.That(buildResponse.Result, Is.EqualTo(result));
        }

        [Test]
        public void BuildResponse_Result_CanBeSetToNull()
        {
            // Arrange
            var buildResponse = new BuildResponse();

            // Act
            buildResponse.Result = null;

            // Assert
            Assert.That(buildResponse.Result, Is.Null);
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
            // Arrange
            var buildResponse = new BuildResponse();

            // Act
            buildResponse.Status = status;

            // Assert
            Assert.That(buildResponse.Status, Is.EqualTo(status));
        }
    }
}
