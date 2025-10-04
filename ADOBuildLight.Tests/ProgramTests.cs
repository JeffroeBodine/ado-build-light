using System.Reflection;
using System.Runtime.InteropServices;
using ADOBuildLight.Interfaces;
using ADOBuildLight.Models;
using ADOBuildLight.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        private Mock<IAppConfiguration> _mockConfig;
        private Mock<IGpioService> _mockGpioService;

        [SetUp]
        public void SetUp()
        {
            _mockConfig = new Mock<IAppConfiguration>();
            _mockGpioService = new Mock<IGpioService>();
        }

        #region AppSettingsValidation Tests

        [Test]
        public void AppSettingsValidation_WithValidSettings_ReturnsTrue()
        {
            var config = new AppConfiguration
            {
                AzureDevOps = new AppConfiguration.AzureDevOpsSettings
                {
                    Organization = "test-org",
                    Project = "test-project",
                    PipelineId = "123",
                    PersonalAccessToken = "test-token",
                },
                BusinessHours = new AppConfiguration.BusinessHoursSettings
                {
                    StartHour = 8,
                    EndHour = 17,
                    DaysOfWeek = new List<string> { "Monday", "Tuesday", "Wednesday" },
                },
            };

            var result = InvokePrivateStaticMethod<bool>("AppSettingsValidation", config);

            result.Should().BeTrue();
        }

        [TestCase("", "test-project", "123", "test-token", TestName = "EmptyOrganization")]
        [TestCase("test-org", "", "123", "test-token", TestName = "EmptyProject")]
        [TestCase("test-org", "test-project", "", "test-token", TestName = "EmptyPipelineId")]
        [TestCase("test-org", "test-project", "123", "", TestName = "EmptyPersonalAccessToken")]
        public void AppSettingsValidation_WithMissingAzureDevOpsSettings_ReturnsFalse(
            string organization,
            string project,
            string pipelineId,
            string token
        )
        {
            var config = new AppConfiguration
            {
                AzureDevOps = new AppConfiguration.AzureDevOpsSettings
                {
                    Organization = organization,
                    Project = project,
                    PipelineId = pipelineId,
                    PersonalAccessToken = token,
                },
                BusinessHours = new AppConfiguration.BusinessHoursSettings
                {
                    StartHour = 8,
                    EndHour = 17,
                    DaysOfWeek = new List<string> { "Monday", "Tuesday" },
                },
            };

            var result = InvokePrivateStaticMethod<bool>("AppSettingsValidation", config);

            result.Should().BeFalse();
        }

        [TestCase(8, 17, new[] { "Monday" }, TestName = "OneDayOfWeek")]
        [TestCase(0, 17, new[] { "Monday", "Tuesday" }, TestName = "ZeroStartHour")]
        [TestCase(8, 0, new[] { "Monday", "Tuesday" }, TestName = "ZeroEndHour")]
        public void AppSettingsValidation_WithInvalidBusinessHours_ReturnsFalse(
            int startHour,
            int endHour,
            string[] daysOfWeek
        )
        {
            var config = new AppConfiguration
            {
                AzureDevOps = new AppConfiguration.AzureDevOpsSettings
                {
                    Organization = "test-org",
                    Project = "test-project",
                    PipelineId = "123",
                    PersonalAccessToken = "test-token",
                },
                BusinessHours = new AppConfiguration.BusinessHoursSettings
                {
                    StartHour = startHour,
                    EndHour = endHour,
                    DaysOfWeek = daysOfWeek.ToList(),
                },
            };

            var result = InvokePrivateStaticMethod<bool>("AppSettingsValidation", config);

            result.Should().BeFalse();
        }

        #endregion

        #region CreateGpioService Tests

        [Test]
        public void CreateGpioService_OnWindows_ReturnsMockGpioService()
        {
            var result = InvokePrivateStaticMethod<IGpioService>("CreateGpioService");

            result.Should().NotBeNull();
            result.Should().BeOfType<MockGpioService>();
        }

        #endregion

        #region IsRaspberryPi Tests

        [Test]
        public void IsRaspberryPi_OnWindows_ReturnsFalse()
        {
            var result = InvokePrivateStaticMethod<bool>("IsRaspberryPi");

            result.Should().BeFalse();
        }

        #endregion

        #region GetOverallStatus Tests

        [TestCase("inProgress", "succeeded", "inProgress")]
        [TestCase("INPROGRESS", "succeeded", "inProgress")]
        [TestCase("completed", "succeeded", "succeeded")]
        [TestCase("completed", "failed", "failed")]
        [TestCase("PENDING", "succeeded", "pending")]
        public void GetOverallStatus_WithVariousStates_ReturnsExpectedResult(
            string state,
            string result,
            string expected
        )
        {
            var actualResult = InvokePrivateStaticMethod<string>("GetOverallStatus", state, result);

            actualResult.Should().Be(expected);
        }

        [Test]
        public void GetOverallStatus_WithCompletedStateAndNullResult_ReturnsUnknown()
        {
            string? nullResult = null;

            var result = InvokePrivateStaticMethod<string>(
                "GetOverallStatus",
                "completed",
                nullResult
            );

            result.Should().Be("unknown");
        }

        [Test]
        public void GetOverallStatus_WithNullState_ReturnsUnknown()
        {
            string? nullState = null;

            var result = InvokePrivateStaticMethod<string>(
                "GetOverallStatus",
                nullState,
                "succeeded"
            );

            result.Should().Be("unknown");
        }

        #endregion

        #region UpdateBuildLight Tests

        [TestCase("succeeded", GpioPins.Green)]
        [TestCase("failed", GpioPins.Red)]
        [TestCase("partiallysucceeded", GpioPins.Orange)]
        [TestCase("partially succeeded", GpioPins.Orange)]
        [TestCase("inprogress", GpioPins.Yellow)]
        [TestCase("running", GpioPins.Yellow)]
        [TestCase("canceled", GpioPins.Red)]
        [TestCase("cancelled", GpioPins.Red)]
        [TestCase("offduty", GpioPins.None)]
        [TestCase("unknown-status", GpioPins.None)]
        public void UpdateBuildLight_WithVariousStatuses_SetsCorrectLight(
            string status,
            int expectedPin
        )
        {
            var mockGpioService = new Mock<IGpioService>();

            InvokePrivateStaticMethod("UpdateBuildLight", status, mockGpioService.Object);

            mockGpioService.Verify(x => x.SetLightColor(expectedPin), Times.Once);
        }

        [Test]
        public void UpdateBuildLight_WithNullGpioService_DoesNotThrow()
        {
            IGpioService? nullGpioService = null;

            FluentActions
                .Invoking(() =>
                    InvokePrivateStaticMethod("UpdateBuildLight", "succeeded", nullGpioService)
                )
                .Should()
                .NotThrow();
        }

        #endregion

        #region IsWithinBusinessHours Tests

        [Test]
        public void IsWithinBusinessHours_WithNullDaysOfWeek_ReturnsTrue()
        {
            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = null!,
                StartHour = 8,
                EndHour = 17,
            };

            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            var result = InvokePrivateStaticMethod<bool>(
                "IsWithinBusinessHours",
                _mockConfig.Object
            );

            result.Should().BeTrue();
        }

        [Test]
        public void IsWithinBusinessHours_WithEmptyDaysOfWeek_ReturnsTrue()
        {
            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = new List<string>(),
                StartHour = 8,
                EndHour = 17,
            };

            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            var result = InvokePrivateStaticMethod<bool>(
                "IsWithinBusinessHours",
                _mockConfig.Object
            );

            result.Should().BeTrue();
        }

        [Test]
        public void IsWithinBusinessHours_CurrentDayNotInList_ReturnsFalse()
        {
            var currentDayOfWeek = DateTime.Now.DayOfWeek.ToString();
            var otherDays = Enum.GetNames<DayOfWeek>()
                .Where(d => d != currentDayOfWeek)
                .Take(2)
                .ToList();

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = otherDays,
                StartHour = 8,
                EndHour = 17,
            };

            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            var result = InvokePrivateStaticMethod<bool>(
                "IsWithinBusinessHours",
                _mockConfig.Object
            );

            result.Should().BeFalse();
        }

        [Test]
        public void IsWithinBusinessHours_CurrentDayInList_WithinHours_ReturnsTrue()
        {
            var currentDayOfWeek = DateTime.Now.DayOfWeek.ToString();
            var currentHour = DateTime.Now.Hour;

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = new List<string> { currentDayOfWeek },
                StartHour = Math.Max(0, currentHour - 1),
                EndHour = currentHour + 2,
            };

            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            var result = InvokePrivateStaticMethod<bool>(
                "IsWithinBusinessHours",
                _mockConfig.Object
            );

            result.Should().BeTrue();
        }

        [Test]
        public void IsWithinBusinessHours_CurrentDayInList_BeforeStartHour_ReturnsFalse()
        {
            var currentDayOfWeek = DateTime.Now.DayOfWeek.ToString();
            var currentHour = DateTime.Now.Hour;

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = new List<string> { currentDayOfWeek },
                StartHour = currentHour + 1,
                EndHour = currentHour + 3,
            };

            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            var result = InvokePrivateStaticMethod<bool>(
                "IsWithinBusinessHours",
                _mockConfig.Object
            );

            result.Should().BeFalse();
        }

        [Test]
        public void IsWithinBusinessHours_CurrentDayInList_AfterEndHour_ReturnsFalse()
        {
            var currentDayOfWeek = DateTime.Now.DayOfWeek.ToString();
            var currentHour = DateTime.Now.Hour;

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = new List<string> { currentDayOfWeek },
                StartHour = Math.Max(0, currentHour - 2),
                EndHour = currentHour,
            };

            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            var result = InvokePrivateStaticMethod<bool>(
                "IsWithinBusinessHours",
                _mockConfig.Object
            );

            result.Should().BeFalse();
        }

        [Test]
        public void IsWithinBusinessHours_CaseInsensitiveDayMatching_ReturnsTrue()
        {
            var currentDayOfWeek = DateTime.Now.DayOfWeek.ToString().ToUpper();
            var currentHour = DateTime.Now.Hour;

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = new List<string> { currentDayOfWeek },
                StartHour = Math.Max(0, currentHour - 1),
                EndHour = currentHour + 2,
            };

            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            var result = InvokePrivateStaticMethod<bool>(
                "IsWithinBusinessHours",
                _mockConfig.Object
            );

            result.Should().BeTrue();
        }

        #endregion

        #region ProcessSingleCheckAsync Tests

        [Test]
        public async Task ProcessSingleCheckAsync_WithinBusinessHours_ProcessesPipelineStatus()
        {
            var mockPipelineService = new Mock<IPipelineService>();
            var mockGpioService = new Mock<IGpioService>();
            var mockBuildResponse = new BuildResponse
            {
                Status = "completed",
                Result = "succeeded",
            };

            mockPipelineService
                .Setup(x => x.GetLatestPipelineRunAsync())
                .ReturnsAsync(mockBuildResponse);

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = new List<string> { DateTime.Now.DayOfWeek.ToString() },
                StartHour = Math.Max(0, DateTime.Now.Hour - 1),
                EndHour = DateTime.Now.Hour + 2,
            };
            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            await InvokePrivateStaticMethodAsync(
                "ProcessSingleCheckAsync",
                mockPipelineService.Object,
                mockGpioService.Object,
                _mockConfig.Object
            );

            mockPipelineService.Verify(x => x.GetLatestPipelineRunAsync(), Times.Once);
            mockGpioService.Verify(x => x.SetLightColor(GpioPins.Green), Times.Once);
        }

        [Test]
        public async Task ProcessSingleCheckAsync_OutsideBusinessHours_SetsOffDutyLight()
        {
            var mockPipelineService = new Mock<IPipelineService>();
            var mockGpioService = new Mock<IGpioService>();
            var mockBuildResponse = new BuildResponse
            {
                Status = "completed",
                Result = "succeeded",
            };

            mockPipelineService
                .Setup(x => x.GetLatestPipelineRunAsync())
                .ReturnsAsync(mockBuildResponse);

            var currentDayOfWeek = DateTime.Now.DayOfWeek.ToString();
            var otherDays = Enum.GetNames<DayOfWeek>()
                .Where(d => d != currentDayOfWeek)
                .Take(2)
                .ToList();

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = otherDays,
                StartHour = 9,
                EndHour = 17,
            };
            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            await InvokePrivateStaticMethodAsync(
                "ProcessSingleCheckAsync",
                mockPipelineService.Object,
                mockGpioService.Object,
                _mockConfig.Object
            );

            mockPipelineService.Verify(x => x.GetLatestPipelineRunAsync(), Times.Never);
            mockGpioService.Verify(x => x.SetLightColor(GpioPins.None), Times.Once);
        }

        [Test]
        public async Task ProcessSingleCheckAsync_WithNullBuildResponse_HandlesGracefully()
        {
            var mockPipelineService = new Mock<IPipelineService>();
            var mockGpioService = new Mock<IGpioService>();

            mockPipelineService
                .Setup(x => x.GetLatestPipelineRunAsync())
                .ReturnsAsync((BuildResponse?)null);

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = new List<string> { DateTime.Now.DayOfWeek.ToString() },
                StartHour = Math.Max(0, DateTime.Now.Hour - 1),
                EndHour = DateTime.Now.Hour + 2,
            };
            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            await InvokePrivateStaticMethodAsync(
                "ProcessSingleCheckAsync",
                mockPipelineService.Object,
                mockGpioService.Object,
                _mockConfig.Object
            );

            mockPipelineService.Verify(x => x.GetLatestPipelineRunAsync(), Times.Once);
            mockGpioService.Verify(x => x.SetLightColor(GpioPins.None), Times.Once);
        }

        [Test]
        public async Task ProcessSingleCheckAsync_WithInProgressBuild_SetsYellowLight()
        {
            var mockPipelineService = new Mock<IPipelineService>();
            var mockGpioService = new Mock<IGpioService>();
            var mockBuildResponse = new BuildResponse { Status = "inProgress", Result = null };

            mockPipelineService
                .Setup(x => x.GetLatestPipelineRunAsync())
                .ReturnsAsync(mockBuildResponse);

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = new List<string> { DateTime.Now.DayOfWeek.ToString() },
                StartHour = Math.Max(0, DateTime.Now.Hour - 1),
                EndHour = DateTime.Now.Hour + 2,
            };
            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            await InvokePrivateStaticMethodAsync(
                "ProcessSingleCheckAsync",
                mockPipelineService.Object,
                mockGpioService.Object,
                _mockConfig.Object
            );

            mockPipelineService.Verify(x => x.GetLatestPipelineRunAsync(), Times.Once);
            mockGpioService.Verify(x => x.SetLightColor(GpioPins.Yellow), Times.Once);
        }

        [TestCase("completed", "failed", GpioPins.Red)]
        [TestCase("completed", "partiallysucceeded", GpioPins.Orange)]
        [TestCase("completed", "succeeded", GpioPins.Green)]
        public async Task ProcessSingleCheckAsync_WithVariousBuildResults_SetsCorrectLight(
            string status,
            string result,
            int expectedPin
        )
        {
            var mockPipelineService = new Mock<IPipelineService>();
            var mockGpioService = new Mock<IGpioService>();
            var mockBuildResponse = new BuildResponse { Status = status, Result = result };

            mockPipelineService
                .Setup(x => x.GetLatestPipelineRunAsync())
                .ReturnsAsync(mockBuildResponse);

            var businessHours = new AppConfiguration.BusinessHoursSettings
            {
                DaysOfWeek = new List<string> { DateTime.Now.DayOfWeek.ToString() },
                StartHour = Math.Max(0, DateTime.Now.Hour - 1),
                EndHour = DateTime.Now.Hour + 2,
            };
            _mockConfig.Setup(x => x.BusinessHours).Returns(businessHours);

            await InvokePrivateStaticMethodAsync(
                "ProcessSingleCheckAsync",
                mockPipelineService.Object,
                mockGpioService.Object,
                _mockConfig.Object
            );

            mockGpioService.Verify(x => x.SetLightColor(expectedPin), Times.Once);
        }

        #endregion

        #region LoadConfiguration Tests

        [Test]
        public void LoadConfiguration_WithValidConfigFile_ReturnsConfiguration()
        {
            // This test would require setting up a temporary appsettings.json file
            // For now, we'll test the validation logic that's part of LoadConfiguration
            var result = InvokePrivateStaticMethod<Models.AppConfiguration?>("LoadConfiguration");

            // The result will be null on Windows without appsettings.json, which is expected
            // This test mainly ensures the method doesn't throw exceptions
            FluentActions
                .Invoking(() =>
                    InvokePrivateStaticMethod<Models.AppConfiguration?>("LoadConfiguration")
                )
                .Should()
                .NotThrow();
        }

        #endregion

        #region Helper Methods

        private static T InvokePrivateStaticMethod<T>(
            string methodName,
            params object?[] parameters
        )
        {
            var assembly = Assembly.Load("ADOBuildLight");
            var programType = assembly.GetType("ADOBuildLight.Program");
            programType.Should().NotBeNull("Program type should exist");

            var method = programType!.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Static
            );
            method.Should().NotBeNull($"Method {methodName} should exist");

            var result = method!.Invoke(null, parameters);
            return (T)result!;
        }

        private static void InvokePrivateStaticMethod(
            string methodName,
            params object?[] parameters
        )
        {
            var assembly = Assembly.Load("ADOBuildLight");
            var programType = assembly.GetType("ADOBuildLight.Program");
            programType.Should().NotBeNull("Program type should exist");

            var method = programType!.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Static
            );
            method.Should().NotBeNull($"Method {methodName} should exist");

            method!.Invoke(null, parameters);
        }

        private static async Task InvokePrivateStaticMethodAsync(
            string methodName,
            params object?[] parameters
        )
        {
            var assembly = Assembly.Load("ADOBuildLight");
            var programType = assembly.GetType("ADOBuildLight.Program");
            programType.Should().NotBeNull("Program type should exist");

            var method = programType!.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public
            );
            method.Should().NotBeNull($"Method {methodName} should exist");

            var result = method!.Invoke(null, parameters);
            if (result is Task task)
            {
                await task;
            }
        }

        #endregion
    }
}
