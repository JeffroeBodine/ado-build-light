using NUnit.Framework;
using ADOBuildLight.Services;
using System;
using System.IO;

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
            // Arrange
            var service = new MockGpioService();

            // Act
            service.Initialize();

            // Assert
            var output = _stringWriter.ToString();
            Assert.That(output, Contains.Substring("[MOCK GPIO] Initializing GPIO pins (simulation mode)"));
            Assert.That(output, Contains.Substring("[MOCK GPIO] Setting light to: OFF"));
            Assert.That(output, Contains.Substring("[MOCK GPIO] Setting light to: RED"));
            Assert.That(output, Contains.Substring("[MOCK GPIO] Setting light to: YELLOW"));
            Assert.That(output, Contains.Substring("[MOCK GPIO] Setting light to: GREEN"));
        }

        [TestCase(GpioPins.Red, "RED")]
        [TestCase(GpioPins.Yellow, "YELLOW")]
        [TestCase(GpioPins.Green, "GREEN")]
        [TestCase(GpioPins.Orange, "ORANGE")]
        [TestCase(GpioPins.None, "OFF")]
        public void SetLightColor_WritesExpectedOutput(int color, string expectedColorName)
        {
            // Arrange
            var service = new MockGpioService();

            // Act
            service.SetLightColor(color);

            // Assert
            var output = _stringWriter.ToString();
            Assert.That(output, Contains.Substring($"[MOCK GPIO] Setting light to: {expectedColorName}"));
        }

        [Test]
        public void Dispose_WritesExpectedOutput()
        {
            // Arrange
            var service = new MockGpioService();

            // Act
            service.Dispose();

            // Assert
            var output = _stringWriter.ToString();
            Assert.That(output, Contains.Substring("[MOCK GPIO] Disposing GPIO controller (simulation mode)"));
        }
    }
}
