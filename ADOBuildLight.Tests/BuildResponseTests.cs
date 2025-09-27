using NUnit.Framework;
using ADOBuildLight.Models;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class BuildResponseTests
    {
        [Test]
        public void BuildResponse_Properties_CanBeSet()
        {
            // Arrange
            var buildResponse = new BuildResponse();
            var status = "completed";
            var result = "succeeded";

            // Act
            buildResponse.Status = status;
            buildResponse.Result = result;

            // Assert
            Assert.That(buildResponse.Status, Is.EqualTo(status));
            Assert.That(buildResponse.Result, Is.EqualTo(result));
        }
    }
}
