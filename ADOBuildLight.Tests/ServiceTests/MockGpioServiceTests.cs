using System;
using System.IO;
using ADOBuildLight.Services;
using FluentAssertions;
using NUnit.Framework;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class MockGpioServiceTests
    {
        private StringWriter _stringWriter;
        private TextWriter _originalOutput;

        [SetUp]
        public void Setup()
        {
            // Skip any artificial delays for faster tests
            RealGpioService.SkipStartupDelays = true;
            _stringWriter = new StringWriter();
            _originalOutput = Console.Out;
            Console.SetOut(_stringWriter);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_originalOutput);
            _stringWriter.Dispose();
        }

        [Test]
        public void Initialize_WritesExpectedOutput()
        {
            var service = new MockGpioService();
            service.Initialize();

            var output = _stringWriter.ToString();
            output.Should().Contain("[MOCK GPIO] Initializing GPIO pins (simulation mode)");
            output.Should().Contain("[MOCK GPIO] Setting light to: OFF");
            output.Should().Contain("[MOCK GPIO] Setting light to: RED");
            output.Should().Contain("[MOCK GPIO] Setting light to: YELLOW");
            output.Should().Contain("[MOCK GPIO] Setting light to: GREEN");
        }

        [TestCase(GpioPins.Red, "RED")]
        [TestCase(GpioPins.Yellow, "YELLOW")]
        [TestCase(GpioPins.Green, "GREEN")]
        [TestCase(GpioPins.Orange, "ORANGE")]
        [TestCase(GpioPins.None, "OFF")]
        public void SetLightColor_WritesExpectedOutput(int color, string expectedColorName)
        {
            var service = new MockGpioService();
            service.SetLightColor(color);

            var output = _stringWriter.ToString();
            output.Should().Contain($"[MOCK GPIO] Setting light to: {expectedColorName}");
        }

        [Test]
        public void Dispose_WritesExpectedOutput()
        {
            var service = new MockGpioService();
            service.Dispose();

            var output = _stringWriter.ToString();
            output.Should().Contain("[MOCK GPIO] Disposing GPIO controller (simulation mode)");
        }
    }
}
