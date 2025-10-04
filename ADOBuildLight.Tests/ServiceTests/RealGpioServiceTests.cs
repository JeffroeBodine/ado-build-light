using NUnit.Framework;
using Moq;
using System.Device.Gpio;
using ADOBuildLight.Services;
using ADOBuildLight.Interfaces;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class RealGpioServiceTests
    {
        private Mock<IGpioController> _mockController;
        private RealGpioService _service;

        [SetUp]
        public void Setup()
        {
            _mockController = new Mock<IGpioController>();
            _service = new RealGpioService(_mockController.Object);
        }

        [Test]
        public void Initialize_OpensPinsAndRunsTestSequence()
        {
            _service.Initialize();

            _mockController.Verify(c => c.OpenPin(GpioPins.Red, PinMode.Output), Times.Once);
            _mockController.Verify(c => c.OpenPin(GpioPins.Yellow, PinMode.Output), Times.Once);
            _mockController.Verify(c => c.OpenPin(GpioPins.Green, PinMode.Output), Times.Once);

            _mockController.Verify(c => c.Write(GpioPins.Red, PinValue.High), Times.AtLeastOnce());
            _mockController.Verify(c => c.Write(GpioPins.Yellow, PinValue.High), Times.AtLeastOnce());
            _mockController.Verify(c => c.Write(GpioPins.Green, PinValue.High), Times.AtLeastOnce());
        }

        [TestCase(GpioPins.Red, 0, 1, 1)]
        [TestCase(GpioPins.Yellow, 1, 0, 1)]
        [TestCase(GpioPins.Green, 1, 1, 0)]
        [TestCase(GpioPins.Orange, 0, 0, 1)]
        [TestCase(GpioPins.None, 1, 1, 1)]
        public void SetLightColor_WritesCorrectPinValues(int color, int red, int yellow, int green)
        {
            _service.Initialize(); // To open pins

            _service.SetLightColor(color);

            _mockController.Verify(c => c.Write(GpioPins.Red, (PinValue)red), Times.AtLeastOnce());
            _mockController.Verify(c => c.Write(GpioPins.Yellow, (PinValue)yellow), Times.AtLeastOnce());
            _mockController.Verify(c => c.Write(GpioPins.Green, (PinValue)green), Times.AtLeastOnce());
        }

        [Test]
        public void Dispose_DisposesController()
        {
            _service.Dispose();

            _mockController.Verify(c => c.Dispose(), Times.Once);
        }
    }
}
